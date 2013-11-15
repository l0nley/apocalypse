using System;
using System.Collections.Generic;
using Sockets.Client;
using Sockets.Definitions;

namespace BinaryProtocolConsole.Commands
{
    public class RtouchCommand :ConsoleCommand
    {

        public RtouchCommand()
        {
            Name = "rtouch";
            ArgsCount = 1;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var fileName = args[0].ToRemotePath(env);
            if(((BinaryProtocolClient)env["activeConnection"]).TouchFile(fileName).Result.OpCode!=OpCodes.Success)
                Console.WriteLine("Error when executing remote command");
            else 
                Console.WriteLine("Remote file {0} created",args[0]);
        }
    }
}