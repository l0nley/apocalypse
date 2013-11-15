using System.Collections.Generic;

namespace BinaryProtocolConsole.Commands
{
    public class RcdCommand :ConsoleCommand
    {
        public RcdCommand()
        {
            ArgsCount = 1;
            Name = "rcd";
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            env["remoteDir"] = args[0].ToRemotePath(env);
        }
    }
}