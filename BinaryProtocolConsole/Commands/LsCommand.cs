using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryProtocolConsole.Commands
{
    public class LsCommand :ConsoleCommand
    {
        public LsCommand()
        {
            Name = "ls";
            ArgsCount = 0;
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var remoteDir = env["localDir"].ToString();
            Console.WriteLine("Directory listing for {0}", remoteDir);
            var di = new DirectoryInfo(remoteDir);
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
                foreach (var dir in di.EnumerateDirectories())
                {
                    Console.WriteLine("{0}\t<DIR>", dir.FullName.Substring(remoteDir.Length));
                }
            }
            if (showFiles)
            {
                foreach (var file in di.EnumerateFiles())
                {
                    Console.WriteLine(file.FullName.Substring(remoteDir.Length));
                }
            }
        }
    }
}