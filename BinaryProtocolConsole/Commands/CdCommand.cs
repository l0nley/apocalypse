using System.Collections.Generic;

namespace BinaryProtocolConsole.Commands
{
    public class CdCommand :ConsoleCommand
    {
        public CdCommand()
        {
            ArgsCount = 1;
            Name = "cd";
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            env["localDir"] = args[0].ToLocalPath(env);
        }
    }
}