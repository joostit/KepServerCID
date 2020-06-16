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

            //FakeDeviceTags myDeviceInKep = new FakeDeviceTags();

            ThreeShapeScannerCid scannerCid = new ThreeShapeScannerCid();

            CidConnector connector = new CidConnector();

            connector.ExportConfiguration(scannerCid.Tags);
            //connector.Start(scannerCid.Tags);


            Task inputWaiter = Task.Run(() =>
            {
                Console.WriteLine("Running. Press any key to exit.");
                Console.ReadKey();
                run = false;
            });

            ushort val = 0;
            while (run)
            {
                Thread.Sleep(1000);
                scannerCid.JobCount.Value = val++;
                scannerCid.IsRunning.Value = val % 2 == 0;
                //myDeviceInKep.WordTag.Value = val++;
            }

            Console.WriteLine("Stopping CID Connector service...");
            connector.Stop();
            Console.WriteLine("Stopped.");
        }

    }
}
