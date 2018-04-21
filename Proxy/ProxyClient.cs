using Proxy.Packet;
using socket.core.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Proxy
{
    public class ProxyClient
    {
        PackClient GameClient;
        PackClient LinkClient;

        public ProxyClient(int _rebuf,uint _flag)
        {
            LinkClient = new PackClient(_rebuf, _flag);
            LinkClient.OnReceive += this.LinkReceive;
            LinkClient.OnConnect += this.LinkConnect;            
        }

        private Timer Heartbeat;

        private void HeartCallBack(object state)
        {
            this.Send(new byte[] { 0, 1, 0, 3 });
        }

        #region Link

        public void Start(string _address, int _port)
        {
            LinkClient.Connect(_address, _port);

            Heartbeat = new Timer(HeartCallBack, null, 0, 8000);
        }

        private void Send(byte[] buffer)
        {
            LinkClient.Send(buffer, 0, buffer.Length);
        }

        private void LinkReceive(int id, byte[] buffer)
        {
            if (buffer[0] == 0x67)
            {
                using (PacketStream packetStream = new PacketStream(buffer))
                {
                    byte a = packetStream.ReadByte();
                    int _id = packetStream.ReadInt();
                    byte[] data = packetStream.ReadBytes();

                    this.GameClient.Send(data, 0, data.Length);
                }
            }
        }

        private void LinkConnect(bool flag)
        {
            Console.WriteLine(string.Format("[{0}] 远程服务器连接{1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), flag));
        }

        #endregion


        #region Game

        private void Game()
        {
            GameClient = new PackClient(56636, 0);            
            GameClient.OnReceive += this.GameReceive;
            GameClient.OnConnect += this.GameConnect;
            GameClient.Connect("192.168.0.3", 29000);
        }

        private void GameReceive(int id, byte[] buffer)
        {
            using (PacketStream packetStream = new PacketStream(buffer))
            {
                packetStream.Insert(0, 0x67);
                packetStream.Insert(1, id);

                this.Send(packetStream.GetBytes());
            }
        }

        private void GameConnect(bool flag)
        {
            Console.WriteLine(string.Format("[{0}] 游戏服务器连接{1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), flag));
        }

        #endregion
    }
}
