using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sockets.Client;
using Sockets.Definitions;

namespace BinaryProtocolConsole.Commands
{
    public class PutCommand :ConsoleCommand
    {
        public PutCommand()
        {
            Name = "put";
            ArgsCount = 1;
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var localFile = args[0].ToLocalPath(env);
            var remoteFile = args[0].ToRemotePath(env);
            var fi = new FileInfo(localFile);
            if (!fi.Exists)
            {
                Console.WriteLine("Local file not exists.");
                return;
            }
            var blockSize = 4096;
            if (args.Length == 3)
            {
                if (args[1] == "-bs")
                {
                    blockSize = Convert.ToInt32(args[2]);
                }
            }
            var client = (BinaryProtocolClient) env["activeConnection"];
            var res = client.GetFileOptions(remoteFile).Result;
            if (res.OpCode != OpCodes.Success)
            {
                Console.WriteLine("Error when getting info about remote file");
                return;
            }
            if (res.Exists)
            {
                Console.WriteLine("Remote file exists. Do you want to overwrite it?[y\\n]");
                if (Console.ReadKey(true).KeyChar != 'y')
                {
                    return;
                }
                if (client.DropFile(remoteFile).Result.OpCode != OpCodes.Success)
                {
                    Console.WriteLine("Error when deleting remote file");
                    return;
                }
            }
            if (client.TouchFile(remoteFile).Result.OpCode != OpCodes.Success)
            {
                Console.WriteLine("Error when trying to recreate remote file");
                return;
            }
            Console.WriteLine("Starting upload of file {0} to {1}. Press <ESC> key for break upload.", localFile,remoteFile);
            var size = fi.Length;
            var buff = new byte[blockSize];
            var start = DateTime.Now;
            using (var st = fi.OpenRead())
            {
                using (var br = new BinaryReader(st))
                {
                    int readed;
                    long offset = 0;
                    while (0 != (readed = br.Read(buff, 0, buff.Length)))
                    {
                        if (Console.KeyAvailable)
                        {
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            {
                                Console.WriteLine("\nUploading breaked by user.");
                                break;
                            }
                        }
                        var sres = client.SetFilePart(remoteFile, 0, buff.Take(readed).ToArray(),true).Result;
                        if (sres.OpCode != OpCodes.Success)
                        {
                            Console.WriteLine("\nUpload error detected. Retrying block.");
                            continue;
                        }
                        offset += readed;
                        Console.Write("\r{0:#00.00} % complete",offset/(double)size*100);
                    }
                }
            }
            var time = DateTime.Now.Subtract(start);
            Console.WriteLine("\nUpload complete in {0}",time);
        }
    }
}