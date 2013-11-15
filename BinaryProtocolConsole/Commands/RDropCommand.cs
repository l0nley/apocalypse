using System;
using System.Collections.Generic;
using Sockets.Client;
using Sockets.Definitions;

namespace BinaryProtocolConsole.Commands
{
    public class RDropCommand :ConsoleCommand
    {
        public RDropCommand()
        {
            Name = "rdrop";
            ArgsCount = 1;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var fileName = args[0].ToRemotePath(env);
            var con = (BinaryProtocolClient) env["activeConnection"];
            Console.WriteLine(
                con.DropFile(fileName).Result.OpCode != OpCodes.Success
                    ? "Remote error when deleting {0}"
                    : "Remote file {0} deleted", fileName);
        }
    }
}