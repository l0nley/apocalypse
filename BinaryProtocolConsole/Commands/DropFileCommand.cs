using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryProtocolConsole.Commands
{
    public class DropFileCommand :ConsoleCommand
    {
        public DropFileCommand()
        {
            Name = "drop";
            ArgsCount = 1;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var recoursive = false;
            if (args.Length == 2)
            {
                if (args[1] == "-r")
                {
                    recoursive = true;
                }
            }
            new DirectoryInfo(args[0].ToLocalPath(env)).Delete(recoursive);
            Console.WriteLine("Directory {0} deleted",args[0].ToLocalPath(env));
        }
    }
}