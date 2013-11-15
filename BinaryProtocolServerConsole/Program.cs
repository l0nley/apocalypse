using System;
using Socket.BinaryCommandProcessor;
using Sockets.Listener;

namespace BinaryProtocolServerConsole
{
    public class Program
    {
        static void Main()
        {
            var list = new BinaryProtocolListener(10137, new BinaryCommandProcessor());
            list.Start();
            Console.WriteLine("Server started. Press <Enter> To exit.");
            Console.ReadLine();
        }
    }
}
