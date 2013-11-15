using System;
using System.Collections.Generic;
using Sockets.Client;
using Sockets.Definitions;

namespace BinaryProtocolConsole
{
    public class RmdCommand :ConsoleCommand
    {
        public RmdCommand()
        {
            Name = "rmd";
            ArgsCount = 1;
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var fileName = args[0].ToRemotePath(env);
            var client = (BinaryProtocolClient) env["activeConnection"];
            if (client.CreateDir(fileName).Result.OpCode != OpCodes.Success)
            {
                Console.WriteLine("Remote create directory {0} failed.",fileName);
                return;
            }
            Console.WriteLine("Remote directory {0} created",fileName);
        }
    }
}