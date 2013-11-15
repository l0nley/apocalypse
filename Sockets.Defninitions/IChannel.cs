using System;
using System.Threading.Tasks;

namespace Sockets.Definitions
{
    public interface IChannel<T>
    {
        IObservable<T> Receiver { get; }
        Task SendAsync(T message);
    }
}
