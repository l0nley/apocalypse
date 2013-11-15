using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sockets.Client;
using Sockets.Definitions;

namespace BinaryProtocolConsole.Commands
{
    public class RunAndWait :ConsoleCommand
    {
        public RunAndWait()
        {
            Name = "runandwait";
            ArgsCount = 1;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            var @params = string.Empty;
            if (args.Length > 1)
            {
                @params = string.Join(" ", args.Skip(1));
            }
            var client = (BinaryProtocolClient)env["activeConnection"];
            if (client.RunAndWait(args[0].ToRemotePath(env), @params).Result.OpCode != OpCodes.Success)
            {
                Console.WriteLine("Error when remote run {0}",args[0].ToRemotePath(env));
                return;
            }
            Console.WriteLine("Remote command execute completed");
        }
    }
}
