using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryProtocolConsole.Commands
{
    public class TouchCommand :ConsoleCommand
    {
        public TouchCommand()
        {
            ArgsCount = 1;
            Name = "touch";
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var fileName = args[0].ToLocalPath(env);
            File.Create(fileName).Close();
            Console.WriteLine("File {0} created",args[0]);
        }
    }
}