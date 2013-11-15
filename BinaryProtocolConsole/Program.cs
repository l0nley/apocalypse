using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BinaryProtocolConsole
{
    public class Program
    {
        private static readonly Dictionary<string, object> Environment = new Dictionary<string, object>();

        private static Dictionary<string,ConsoleCommand> _commands = new Dictionary<string, ConsoleCommand>();

        static void Main()
        {
            string s;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("Loading commands...");
            _commands = Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(l => l.IsSubclassOf(typeof (ConsoleCommand)))
                                .Select(l => (ConsoleCommand) Activator.CreateInstance(l)).ToDictionary(l => l.Name);
            Console.WriteLine("Loading commands complete.");
            Console.WriteLine("Welcome to console master");
            Console.Write(Prompt);
            while ("exit" != (s = Console.ReadLine()))
            {
                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }
                if (s == "lscmd")
                {
                    foreach (var cmd in _commands)
                    {
                        Console.WriteLine(cmd.Key);
                    }
                    Console.Write(Prompt);
                    continue;
                }
                var ss = s.Split(' ');
                if (!_commands.ContainsKey(ss[0]))
                {
                    Console.Write("Command not found\n"+Prompt);
                    continue;
                }
                var ccmd = _commands[ss[0]];
                if (ccmd.ArgsCount > (ss.Length - 1))
                {
                    Console.Write("Not enough args, needed {0}\n{1}", ccmd.ArgsCount,Prompt);
                    continue;
                }
                try
                {
                    ccmd.ExecuteCommand(ss.Skip(1).ToArray(), Environment);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception during executing command : {0}\n{1}",e.Message,e.StackTrace);
                }
                Console.Write(Prompt);
            }
        }

        static string Prompt
        {
            get
            {
                return string.Format("[R:{0}]>",
                                     Environment.ContainsKey("remoteDir") ? Environment["remoteDir"].ToString() : "???");
            }
        } 
    }
}
