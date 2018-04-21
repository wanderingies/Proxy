using System;

namespace Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var proxy = new ProxyClient(56636, 0x103f);
            proxy.Start("127.0.0.1", 3000);
        }
    }
}
