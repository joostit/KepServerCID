using CidaRefImplCsharp;
using System;

namespace KepServer.Cid.CidTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started");

            CidConnector connector = new CidConnector();

            connector.Run(args);

        }
    }
}
