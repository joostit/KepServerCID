using KepServer.CidLib;
using KepServer.CidLib.Tags;
using KepServer.CidLib.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KepServer.Cid.CidTester
{
    class Program
    {
        static void Main(string[] args)
        {
            bool run = true;
            Console.WriteLine("Started");

            FakeDeviceTags fakeDevice = new FakeDeviceTags();

            CidConnector connector = new CidConnector();

            //connector.ExportConfiguration(fakeDevice.Tags);
            connector.Start(fakeDevice.Tags);


            Task inputWaiter = Task.Run(() =>
            {
                Console.WriteLine("Running. Press any key to exit.");
                Console.ReadKey();
                run = false;
            });


            while (run)
            {
                Thread.Sleep(500);

                fakeDevice.WordTag.Value = (ushort)DateTime.Now.Second;
            }



            Console.WriteLine("Stopping CID Connector service...");
            connector.Stop();
            Console.WriteLine("Stopped.");
        }

    }
}
