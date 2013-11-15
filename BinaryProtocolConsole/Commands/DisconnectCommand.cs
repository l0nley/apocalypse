using System;
using System.Collections.Generic;
using Sockets.Client;

namespace BinaryProtocolConsole.Commands
{
    public class DisconnectCommand :ConsoleCommand
    {
        public DisconnectCommand()
        {
            Name = "disconnect";
            ArgsCount = 0;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            if (!env.ContainsKey("activeConnection"))
            {
                Console.WriteLine("There is not active connection");
            }
            var con = (BinaryProtocolClient) env["activeConnection"];
            con.Disconnect();
            env.Remove("activeConnection");
            env.Remove("currentHost");
            env.Remove("currentPort");
        }
    }
}