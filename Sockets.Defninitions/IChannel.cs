using System;
using System.Threading.Tasks;

namespace Apocalypse.Sockets.Definitions
{
    public interface IChannel<T>
    {
        IObservable<T> Receiver { get; }
        Task SendAsync(T message);
    }
}
