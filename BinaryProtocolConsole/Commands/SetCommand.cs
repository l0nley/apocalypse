using System.Collections.Generic;

namespace BinaryProtocolConsole.Commands
{
    public class SetCommand :ConsoleCommand
    {
        public SetCommand()
        {
            Name = "set";
            ArgsCount = 2;
        }

        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            if (env.ContainsKey(args[0]))
            {
                env[args[0]] = args[1];
            }
            else
            {
                env.Add(args[0],args[1]);
            }
        }
    }
}