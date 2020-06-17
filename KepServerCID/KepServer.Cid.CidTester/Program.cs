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

            FakeDeviceTags myDeviceInKep = new FakeDeviceTags();

            ThreeShapeScannerCid scannerCid = new ThreeShapeScannerCid();

            CidConnector connector = new CidConnector();

            //connector.ExportConfiguration(myDeviceInKep.Tags);
            connector.Start(myDeviceInKep.Tags);


            Task inputWaiter = Task.Run(() =>
            {
                Console.WriteLine("Running. Press any key to exit.");
                Console.ReadKey();
                run = false;
            });

            byte val = 0;
            while (run)
            {
                Thread.Sleep(1000);
                setFakeValues(myDeviceInKep, val);

                val++;
            }

            Console.WriteLine("Stopping CID Connector service...");
            connector.Stop();
            Console.WriteLine("Stopped.");
        }


        private static void setFakeValues(FakeDeviceTags device, byte val)
        {
            device.WordTag.Value = val;
            device.BoolTag.Value = val % 2 == 0;
            device.CharTag.Value = (sbyte) val;
            device.ByteTag.Value = val;
            device.ShortTag.Value = val;
            device.LongTag.Value = val;
            device.DWordTag.Value = val;
            device.FloatTag.Value = val;
            device.DoubleTag.Value = val;
            device.DateTag.Value = DateTime.Parse($"2021-03-04T12:34:56.{val}");
            device.StringTag.Value = $"Value is {val}ms";

            device.BoolArrayTag[2,2] = val % 2 == 0;


            device.StringArrayTag[0, 2] = "Fake: " + val.ToString();
        }
        

    }
}
