using System;
using System.Collections.Generic;
using Sockets.Client;

namespace BinaryProtocolConsole.Commands
{
    public class ConnectCommand :ConsoleCommand
    {
        public ConnectCommand()
        {
            Name = "connect";
            ArgsCount = 2;
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            if (env.ContainsKey("activeConnection"))
            {
                Console.WriteLine("There is already connected");
                return;
            }
            var con = new BinaryProtocolClient(args[0], Convert.ToInt32(args[1]));
            con.Connect();
            env.Add("currentHost",args[0]);
            env.Add("currentPort",args[1]);
            env.Add("activeConnection",con);
        }

    }
}