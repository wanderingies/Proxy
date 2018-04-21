using System;
using System.Collections.Generic;
using System.Text;

namespace Proxy.Packet
{
    public class PacketStream : OctetsStream
    {
        public PacketStream() { }

        public PacketStream(int count)
            : base(count) { }

        public PacketStream(byte[] buffer)
            : base(buffer) { }

        public PacketStream(OctetsStream packetStream)
            : base(packetStream) { }

        private int pos = 0;

        public int Position()
        {
            return this.pos;
        }

        public int Position(int pos)
        {
            this.pos = pos;
            return this.pos;
        }

        public byte ReadByte()
        {
            if (this.pos + 1 <= Size())
                return GetByte(this.pos++);

            throw new Exception("");
        }

        public byte[] ReadBytes()
        {
            byte[] result;
            if (Size() - this.pos >= 0)
            {
                result = new byte[Size() - this.pos];
                Array.Copy(buffer, this.pos, result, 0, result.Length);

                this.pos = Size();
                return result;
            }

            throw new Exception("");
        }

        public byte[] ReadBytes(int size)
        {
            byte[] result;
            if (this.pos + size <= Size())
            {
                result = new byte[size];
                Array.Copy(buffer, this.pos, result, 0, result.Length);
                this.pos += size;
                return result;
            }

            throw new Exception("");
        }

        public int ReadInt()
        {
            if (this.pos + 4 <= Size())
            {
                int i = GetByte(this.pos++);
                int j = GetByte(this.pos++);
                int k = GetByte(this.pos++);
                int m = GetByte(this.pos++);
                return (i & 0xFF) << 24 | (j & 0xFF) << 16 | (k & 0xFF) << 8 | (m & 0xFF) << 0;
            }

            throw new Exception("");
        }

        public void Write(byte value)
        {
            Push_back(value);
        }

        public void Write(int value)
        {
            Write((byte)(value >> 24));
            Write((byte)(value >> 16));
            Write((byte)(value >> 8));
            Write((byte)(value));
        }
    }
}
