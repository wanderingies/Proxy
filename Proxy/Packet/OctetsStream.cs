using System;

namespace Proxy.Packet
{
    public class OctetsStream : ICloneable,IDisposable
    {
        public OctetsStream()
        {
            Reserve(DEFAULT_SIZE);
        }

        public OctetsStream(int size)
        {
            Reserve(size);
        }

        public OctetsStream(byte[] buffer)
        {
            Replace(buffer);
        }

        public OctetsStream(byte[] buffer, int count)
        {
            this.count = count;
            this.buffer = buffer;
        }

        public OctetsStream(OctetsStream stream)
        {
            Replace(stream);
        }

        public OctetsStream(byte[] buffer, int index, int length)
        {
            Replace(buffer, index, length);
        }

        public OctetsStream(OctetsStream stream, int index, int length)
        {
            Replace(stream, index, length);
        }

        public int count = 0;
        public byte[] buffer;
        private static int DEFAULT_SIZE = 128;

        private byte[] Roundup(int val)
        {
            int i = 16;
            while (val > i)
            {
                i <<= 1;
            }
            return new byte[i];
        }

        public void Reserve(int val)
        {
            if (this.buffer == null)
            {
                this.buffer = Roundup(val);
            }
            else if (val > this.buffer.Length)
            {
                byte[] buffer = Roundup(val);
                Array.Copy(this.buffer, 0, buffer, 0, this.count);
                this.buffer = buffer;
            }
        }

        public OctetsStream Replace(byte[] buffer)
        {
            return Replace(buffer, 0, buffer.Length);
        }

        public OctetsStream Replace(byte[] buffer, int index, int length)
        {
            Reserve(length);
            Array.Copy(buffer, index, this.buffer, 0, length);
            this.count = length;
            return this;
        }

        public OctetsStream Replace(OctetsStream stream)
        {
            return Replace(stream.buffer, 0, stream.count);
        }

        public OctetsStream Replace(OctetsStream stream, int index, int length)
        {
            return Replace(stream.buffer, index, length);
        }

        public int Size()
        {
            return this.count;
        }

        public int Capacity()
        {
            return this.buffer.Length;
        }

        public OctetsStream Clear()
        {
            this.count = 0;
            return this;
        }

        public OctetsStream Resize(int val)
        {
            Reserve(val);
            this.count = val;
            return this;
        }

        public OctetsStream Swap(OctetsStream stream)
        {
            int i = this.count;
            this.count = stream.count;
            stream.count = i;
            byte[] data = stream.buffer;
            stream.buffer = this.buffer;
            this.buffer = data;
            return this;
        }

        public OctetsStream Push_back(byte buffer)
        {
            Reserve(this.count + 1);
            this.buffer[(this.count++)] = buffer;
            return this;
        }

        public OctetsStream Erase(int desIndex, int souIndex)
        {
            Array.Copy(this.buffer, souIndex, this.buffer, desIndex, this.count - souIndex);

            this.count -= souIndex - desIndex;
            return this;
        }

        public OctetsStream Insert(int index,byte value)
        {
            byte[] v = new byte[] { value };
            return Insert(index, v);
        }

        public OctetsStream Insert(int index, int value)
        {
            byte[] iv = new byte[] {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)(value) };

            return Insert(index, iv);
        }

        public OctetsStream Insert(int index, byte[] buffer)
        {
            return Insert(index, buffer, 0, buffer.Length);
        }

        public OctetsStream Insert(int index, OctetsStream stream)
        {
            return Insert(index, stream.buffer, 0, stream.Size());
        }

        public OctetsStream Insert(int index, byte[] buffer, int souIndex, int lemgth)
        {
            Reserve(this.count + lemgth);
            Array.Copy(this.buffer, index, this.buffer, index + lemgth, this.count - index);

            Array.Copy(buffer, souIndex, this.buffer, index, lemgth);

            this.count += lemgth;
            return this;
        }

        public OctetsStream Insert(int index, OctetsStream stream, int souIndex, int length)
        {
            return Insert(index, stream.buffer, souIndex, length);
        }

        public byte GetByte(int index)
        {
            return this.buffer[index];
        }

        public void SetByte(int index, byte value)
        {
            this.buffer[index] = value;
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[this.count];
            Array.Copy(this.buffer, 0, buffer, 0, this.count);
            return buffer;
        }

        public object Clone()
        {
            return (object)new OctetsStream();
        }

        public void Dispose()
        {
            this.count = 0;
            this.buffer = new byte[] { };
        }
    }
}
