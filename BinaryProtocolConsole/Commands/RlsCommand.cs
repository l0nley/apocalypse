using System;
using System.Collections.Generic;
using Sockets.Client;
using Sockets.Definitions;

namespace BinaryProtocolConsole.Commands
{
    public class RlsCommand :ConsoleCommand
    {
        public RlsCommand()
        {
            Name = "rls";
            ArgsCount = 0;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var remoteDir = env["remoteDir"].ToString();
            var con = (BinaryProtocolClient) env["activeConnection"];
            var res = con.ListDirectory(remoteDir).Result;
            Console.WriteLine("Directory listing for {0}",remoteDir);
            if (res.OpCode != OpCodes.Success)
            {
                Console.WriteLine("Remote error when executing command");
                return;
            }
            var showFiles = true;
            var showDirs = true;
            foreach (var s in args)
            {
                switch (s)
                {
                    case "-f":
                        showDirs = false;
                        break;
                    case "-d":
                        showFiles = false;
                        break;
                }
            }
            if (showDirs)
            {
                foreach (var dir in res.Directories)
                {
                    Console.WriteLine("{0}\t<DIR>", dir.Substring(remoteDir.Length));
                }
            }
            if (showFiles)
            {
                foreach (var file in res.Files)
                {
                    Console.WriteLine(file.Substring(remoteDir.Length));
                }
            }
        }
    }
}