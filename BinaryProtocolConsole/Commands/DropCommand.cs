using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryProtocolConsole.Commands
{
    public class DropCommand :ConsoleCommand
    {
        public DropCommand()
        {
            Name = "drop";
            ArgsCount = 1;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            new FileInfo(args[0].ToLocalPath(env)).Delete();
            Console.WriteLine("Local file {0} deleted",args[0].ToLocalPath(env));
        }
    }
}