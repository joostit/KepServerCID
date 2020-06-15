using KepServer.CidLib;
using KepServer.CidLib.Tags;
using KepServer.CidLib.Types;
using System;

namespace KepServer.Cid.CidTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started");

            TagsCollection tags = new TagsCollection();
            tags.AddDevice("MyTestDevice", "DeviceIdblabla")
                .AddTag(new WordTag("aWordTag", "Some fake tag"))
                .AddTag(new BoolTag("aBoolTag", "a Boolean tag"))
                .AddTag(new ByteTag("aByteTag", "a Byte tag"))
                .AddTag(new ShortTag("aShortTag", "a Short tag"))
                .AddTag(new LongTag("aLongTag", "a long tag"))
                .AddTag(new DWordTag("aDWordTag", "a DWORD tag"))
                .AddTag(new FloatTag("aFloatTag", "a Float tag"))
                .AddTag(new DoubleTag("aDoubleTag", "a Double tag"))
                .AddTag(new DateTag("aDateTag", "a Date tag"))
                .AddTag(new StringTag("aStringTag", "a String tag", 16));


            CidConnector connector = new CidConnector();

            bool doExport = false;

            connector.Run(tags, doExport);

        }
    }
}
