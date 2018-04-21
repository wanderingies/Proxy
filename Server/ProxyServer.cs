using Proxy.Packet;
using Proxy.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proxy
{
    public class ProxyServer
    {
        PackServer LinkServer;
        PackServer GameServer;

        private int currentGame = 0;
        private Dictionary<int, int> Ids = null;

        public ProxyServer(int _mun, int _rebuf, int _overtime, uint _flag)
        {
            Ids = new Dictionary<int, int>();

            GameServer = new PackServer(_mun, _rebuf, _overtime, _flag);
            GameServer.OnAccept += this.OnAccept;
            GameServer.OnReceive += this.OnReceive;
        }

        #region Game

        public void Start(int port)
        {
            GameServer.Start(port);
        }        

        private void OnAccept(int id)
        {
            if (currentGame == 0)
            {
                currentGame = id;
                this.Link();
                Console.WriteLine(string.Format("[{0}] 服务端已连接！ 序号({1})", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), id));
            }
        }

        private void Send(int id, byte[] buffer)
        {
            GameServer.Send(id, buffer, 0, buffer.Length);
        }

        private void OnReceive(int id, byte[] buffer)
        {
            if (buffer[0] == 0x67)
            {
                using (PacketStream packetStream = new PacketStream(buffer))
                {
                    byte a = packetStream.ReadByte();
                    int _id = packetStream.ReadInt();
                    byte[] data = packetStream.ReadBytes();

                    this.LinkServer.Send(_id, data, 0, data.Length);
                }
            }
        }

        #endregion

        #region Link

        private void Link()
        {
            LinkServer = new PackServer(2000, 65536, 10, 0);
            LinkServer.OnAccept += this.LinkAccept;
            LinkServer.OnReceive += this.LinkReceive;

            LinkServer.Start(29000);
        }

        private void LinkAccept(int id)
        {
            if (!Ids.ContainsKey(id) && this.currentGame != 0)
            {
                Ids.Add(id, currentGame);

                using (PacketStream packetStream = new PacketStream(new byte[] { 0 }))
                {
                    packetStream.Insert(0, (byte)0x67);
                    packetStream.Insert(1, id);

                    var data = packetStream.ReadBytes();
                    this.Send(id, data);
                }

                Console.WriteLine(string.Format("[{0}] 客户端已连接！ 序号({1})", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), id));
            }
        }

        private void LinkReceive(int id, byte[] buffer)
        {
            using (PacketStream packetStream = new PacketStream(buffer))
            {
                packetStream.Insert(0, (byte)0x67);
                packetStream.Insert(1, id);

                var data = packetStream.ReadBytes();
                this.Send(id, data);
            }
        }

        #endregion

    }
}
