using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryProtocolConsole.Commands
{
    public class MdCommand:ConsoleCommand
    {
        public MdCommand()
        {
            Name = "md";
            ArgsCount = 1;
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var fileName = args[0].ToLocalPath(env);
            var dir = new DirectoryInfo(fileName);
            dir.Create();
            Console.WriteLine("Directory {0} created",fileName);
        }
    }
}