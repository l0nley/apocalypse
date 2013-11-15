using System.Collections.Generic;

namespace BinaryProtocolConsole.Commands
{
    public class InitCommand :ConsoleCommand
    {
        public InitCommand()
        {
            Name = "init";
            ArgsCount = 0;
        }
        public override void ExecuteCommand(string[] args, Dictionary<string, object> env)
        {
            env.Add("remoteDir","C:\\");
            env.Add("localDir", "C:\\");
        }
    }
}