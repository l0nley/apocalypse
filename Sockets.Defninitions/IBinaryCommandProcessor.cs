namespace Apocalypse.Sockets.Definitions
{
    public interface IBinaryCommandProcessor
    {
        void ProcessCommand(BinaryMessage sock, IChannel<BinaryMessage> proto);
    }
}