using System;
using System.Collections.Generic;
using Sockets.Client;
using Sockets.Definitions;

namespace BinaryProtocolConsole.Commands
{
    public class RrdCommand :ConsoleCommand
    {

        public RrdCommand()
        {
            Name = "rrd";
            ArgsCount = 1;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var fileName = args[0].ToRemotePath(env);
            var rec = args.Length == 2 && args[1] == "-r";
            var client = (BinaryProtocolClient) env["activeConnection"];
            if(client.DropDirectory(fileName,rec).Result.OpCode!=OpCodes.Success)
                Console.WriteLine("Remote directory {0} deleted",fileName);
            else
            {
                Console.WriteLine("Error when deleting remote directory");
            }
        }
    }
}