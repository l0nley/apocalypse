using System;

namespace Apocalypse.Sockets.Definitions
{
    public class BinaryMessage
    {
        public Guid SequenceId { get; set; }

        public long Length { get; set; }

        public long OpCode { get; set; }

        public byte[] Payload { get; set; }
    }
}