using System;
using System.Collections.Generic;
using System.Linq;
using Sockets.Client;
using Sockets.Definitions;

namespace BinaryProtocolConsole.Commands
{
    public class RunNoWait :ConsoleCommand
    {
        public RunNoWait()
        {
            Name = "runnowait";
            ArgsCount = 1;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var client = (BinaryProtocolClient) env["activeConnection"];
            var @params = string.Empty;
            if (args.Length > 1)
            {
                @params = string.Join(" ", args.Skip(1));
            }
            if (client.RunNoWait(args[0].ToRemotePath(env), @params).Result.OpCode != OpCodes.Success)
            {
                Console.WriteLine("Error when executing remote command {0}", args[0].ToRemotePath(env));
            }
            else
            {
                Console.WriteLine("Remote command executed");
            }
        }
    }
}