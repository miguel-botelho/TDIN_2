using System;
using System.Runtime.Remoting;

namespace BookStoreWarehouseServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server starting..");
            RemotingConfiguration.Configure("BookStoreWarehouseServer.exe.config", false);
            Console.WriteLine("Press Return to terminate.");
            Console.ReadLine();
        }
    }
}
