using System;
using System.Collections.Generic;
using System.IO;
using Sockets.Client;
using Sockets.Definitions;

namespace BinaryProtocolConsole.Commands
{
    public class GetCommand :ConsoleCommand
    {
        public GetCommand()
        {
            Name = "get";
            ArgsCount = 1;
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var localFile = args[0].ToLocalPath(env);
            var remote = args[0].ToRemotePath(env);
            var blockSize = 4096;
            var client = (BinaryProtocolClient) env["activeConnection"];
            var remoteres = client.GetFileOptions(remote).Result;
            if (args.Length == 3)
            {
                if (args[1] == "-bs")
                {
                    blockSize = Convert.ToInt32(args[2]);
                }
            }
            if (!remoteres.Exists)
            {
                Console.WriteLine("Remote file {0} not exists",remote);
                return; 
            }

            var fi = new FileInfo(localFile);
            if (fi.Exists)
            {
                Console.WriteLine("Local file {0} exists. Do you want to overwrite?[y/n]",localFile);
                if (Console.ReadKey(true).KeyChar != 'y')
                {
                    return;
                }
                fi.Delete();
            }
            Console.WriteLine("Downloading file {0} to {1}. Press <ESC> key to break download.",remote,localFile);
            var start = DateTime.Now;
            using (var fs = fi.Create())
            {
                using (var sw = new BinaryWriter(fs))
                {
                    long offset = 0;
                    while (offset<remoteres.Size)
                    {
                        if (Console.KeyAvailable)
                        {
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            {
                                Console.WriteLine("\nDownload breaked by user.");
                                break;
                            }
                        }
                        var rr = client.GetFilePart(remote, offset, blockSize).Result;
                        if (rr.OpCode != OpCodes.Success)
                        {
                            Console.WriteLine("\nRemote read file failed. Retrying block.");
                            continue;
                        }
                        sw.Write(rr.Payload,0,rr.Payload.Length);
                        offset += rr.Payload.Length;
                        Console.Write("\r{0:#00.00} % complete", offset / (double)remoteres.Size * 100);
                    }
                }
            }
            var time = DateTime.Now.Subtract(start);
            Console.WriteLine("\nDownload complete in {0}",time);

        }
    }
}