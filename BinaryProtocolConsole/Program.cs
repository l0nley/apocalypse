using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinaryProtocolConsole
{
    public class Program
    {
        private static readonly Dictionary<string, object> Environment = new Dictionary<string, object>();

        private static Dictionary<string,ConsoleCommand> _commands = new Dictionary<string, ConsoleCommand>();

        static int  Main(string[] args)
        {
            string s;
            Environment.Add("breakScriptAfterError",true);
            Environment.Add("autoExitAfterExecutingSuccess",false);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("Loading commands...");
            _commands = Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(l => l.IsSubclassOf(typeof(ConsoleCommand)))
                                .Select(l => (ConsoleCommand)Activator.CreateInstance(l)).ToDictionary(l => l.Name);
            Console.WriteLine("Loading commands complete.");
            Console.WriteLine("Welcome to console master");

            if (args.Length != 0)
            {
                var fileName = args[0];
                int code;
                if ((code=ExecuteFile(fileName)) != 0)
                {
                    return code;
                }
                var exitAfterSuccess = (bool)Environment["autoExitAfterExecutingSuccess"];
                if (exitAfterSuccess)
                {
                    return 0;
                }
            }
           
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

            return 0;
        }

        private static int ExecuteFile(string fileName)
        {
            var breakAfterError = (bool) Environment["breakScriptAfterError"];
            try
            {
                string[] commands;
                using (TextReader fs = File.OpenText(fileName))
                {
                    commands = fs.ReadToEnd().Split(new [] {"\n"},StringSplitOptions.RemoveEmptyEntries);
                }
                foreach (var command in commands)
                {
                    var ss = command.Split(' ');
                    var ccmd = ss[0];
                    var @params = ss.Skip(1).ToArray();
                    if (_commands.ContainsKey(ccmd))
                    {
                        var ocmd = _commands[ccmd];
                        if (@params.Length < ocmd.ArgsCount)
                        {
                            Console.WriteLine("Not enough params for command {0}",ccmd);
                            if (breakAfterError)
                            {
                                return 3;
                            }
                            continue;
                        }
                        try
                        {
                            ocmd.ExecuteCommand(@params,Environment);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error in command {0}\n{1}\n{2}",ccmd,e.Message,e.StackTrace);
                            if (breakAfterError)
                            {
                                return 4;
                            }
                        }
                    }
                    else
                    {
                        if (breakAfterError)
                        {
                            return 2;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when executing file \n{0}\n{1}",e.Message,e.StackTrace);
                return 1;
            }
            return 0;
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
