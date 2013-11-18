namespace Apocalypse.Sockets.Definitions
{
    public interface IBinaryCommandProcessor
    {
        void ProcessCommand(BinaryMessage sock, BinaryProtoChannel proto);
    }
}