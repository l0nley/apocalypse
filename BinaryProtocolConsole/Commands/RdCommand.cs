using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryProtocolConsole.Commands
{
    public class RdCommand : ConsoleCommand
    {
        public RdCommand()
        {
            Name = "rd";
            ArgsCount = 1;
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var rec = args.Length == 2 && args[1] == "-r";
            new DirectoryInfo(args[0].ToLocalPath(env)).Delete(rec);
            Console.WriteLine("Directory {0} deleted",args[0].ToLocalPath(env));
        }
    }
}