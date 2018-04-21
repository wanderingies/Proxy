using Proxy.Packet;
using System;

namespace Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = new ProxyServer(2000, 65536, 10, 0x103f);
            proxy.Start(3000);

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
