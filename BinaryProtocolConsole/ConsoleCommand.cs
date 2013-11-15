using System.Collections.Generic;

namespace BinaryProtocolConsole
{
    public abstract class ConsoleCommand
    {
        public string Name { get; protected set; }
        public int ArgsCount { get; protected set; }
        public abstract void ExecuteCommand(string[] args, Dictionary<string,object> env);
    }
}