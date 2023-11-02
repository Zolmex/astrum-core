using Common.Utilities.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameServer.Game.Net.Messaging
{
    public readonly struct StreamWriteInfo // Packet bytes are already written to the packet's stream, here we save the info to know where each packet ends and begins
    {
        public readonly int Offset;
        public readonly int Length;

        public StreamWriteInfo(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }
    }

    public abstract class OutgoingPacket : Packet
    {
        public readonly NetworkWriter Writer;
        private readonly MemoryStream _stream = new MemoryStream();
        private readonly ConcurrentQueue<StreamWriteInfo> _pending = new ConcurrentQueue<StreamWriteInfo>();
        public int PendingCount;

        public OutgoingPacket()
        {
            Writer = new NetworkWriter(_stream);
        }

        public void AddPending(StreamWriteInfo writeInfo)
        {
            Interlocked.Increment(ref PendingCount);
            _pending.Enqueue(writeInfo);
        }

        public int Write(byte[] buff, int offset)
        {
            // Get body
            if (PendingCount == 0)
                return 0;

            var totalLength = 0;
            while (_pending.TryDequeue(out var writeInfo))
            {
                // Check to see if the buffer is big enough
                var packetOffset = writeInfo.Offset;
                var bodyLength = writeInfo.Length;
                var packetLength = bodyLength + 5;

                if (packetLength > buff.Length - offset)
                {
                    _pending.Enqueue(writeInfo); // Couldn't fit within the network buffer, send it later
                    break;
                }

                lock (this)
                {
                    // Write body to the output buffer
                    Buffer.BlockCopy(_stream.GetBuffer(), packetOffset, buff, offset + 5, bodyLength);
                    // Write body length at the beginning of the buffer
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(packetLength)), 0, buff, offset, 4);

                    // Reset the network stream if the list is empty
                    if (Interlocked.Decrement(ref PendingCount) == 0)
                        _stream.SetLength(0);
                }

                // Write packet id next to the body length in the buffer
                buff[offset + 4] = (byte)ID;
                totalLength += packetLength;
                offset += packetLength;
            }
            return totalLength;
        }
    }
}
