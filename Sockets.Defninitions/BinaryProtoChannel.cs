using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveSockets;

namespace Sockets.Definitions
{
    public class BinaryProtoChannel :IChannel<BinaryMessage>,IDisposable
    {
        private readonly ReactiveSocket _socket;
        private readonly string _encryptionKey;
        private const int LongSize = sizeof (long);
        private const int IntSize = sizeof (int);
        private readonly int _guidSize;
        private readonly int _headerSize;
       
        public BinaryProtoChannel(ReactiveSocket socket,string encryptionKey)
        {
            _guidSize = Guid.NewGuid().ToByteArray().Length;
            _headerSize = _guidSize + LongSize+IntSize;
            _socket = socket;
            _encryptionKey = encryptionKey;
            Receiver = from header in _socket.Receiver.Buffer(_headerSize)
                       let headerArr = header.ToArray()
                       let seqId = new Guid(headerArr.Take(_guidSize).ToArray())
                       let opCode = BitConverter.ToInt64(headerArr, _guidSize)
                       let length = BitConverter.ToInt32(headerArr, _guidSize + LongSize)
                       let payload = _socket.Receiver.Take(length).ToEnumerable().ToArray()
                       select new BinaryMessage
                           {
                               Length = length,
                               OpCode = opCode,
                               SequenceId = seqId,
                               Payload = new Cryptor(_encryptionKey).Decrypt(payload)
                           };
        }

        

        public IObservable<BinaryMessage> Receiver { get; private set; }
        public Task SendAsync(BinaryMessage message)
        {
            return _socket.SendAsync(Convert(message));
        }

        private byte[] Convert(BinaryMessage message)
        {
            var seqId = message.SequenceId.ToByteArray();
            var opcode = BitConverter.GetBytes(message.OpCode);
            var res = new Cryptor(_encryptionKey).Crypt(message.Payload);
            return seqId.Concat(opcode).Concat(BitConverter.GetBytes(res.Length)).Concat(res).ToArray();
        }

        public void Dispose()
        {
            _socket.Dispose();
        }
    }
}