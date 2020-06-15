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
                .AddTag(new CharTag("aCharTag", "a Character tag"))
                .AddTag(new ByteTag("aByteTag", "a Byte tag"))
                .AddTag(new ShortTag("aShortTag", "a Short tag"))
                .AddTag(new LongTag("aLongTag", "a long tag"))
                .AddTag(new DWordTag("aDWordTag", "a DWORD tag"))
                .AddTag(new FloatTag("aFloatTag", "a Float tag"))
                .AddTag(new DoubleTag("aDoubleTag", "a Double tag"))
                .AddTag(new DateTag("aDateTag", "a Date tag"))
                .AddTag(new StringTag("aStringTag", "a String tag", 16))
                .AddTag(new BoolArrayTag("aBoolArrayTag", 3, 4, "a Boolean array tag of 3 x 4"))
                .AddTag(new CharArrayTag("aCharArrayTag", 3, 4, "a Char array tag of 3 x 4"))
                .AddTag(new ByteArrayTag("aByteArrayTag", 3, 4, "a Byte array tag of 3 x 4"))
                .AddTag(new ShortArrayTag("aShortArrayTag", 3, 4, "a Short array tag of 3 x 4"))
                .AddTag(new WordArrayTag("aWordArrayTag", 3, 4, "a Word array tag of 3 x 4"))
                .AddTag(new LongArrayTag("aLongArrayTag", 3, 4, "a Long array tag of 3 x 4"))
                .AddTag(new DWordArrayTag("aDWordArrayTag", 3, 4, "a DWORD array tag of 3 x 4"))
                .AddTag(new FloatArrayTag("aFloatArrayTag", 3, 4, "a Float array tag of 3 x 4"))
                .AddTag(new DoubleArrayTag("aDoubleArrayTag", 3, 4, "a Double array tag of 3 x 4"))
                .AddTag(new DateArrayTag("aDateArrayTag", 3, 4, "a Date array tag of 3 x 4"))
                .AddTag(new StringArrayTag("aStringArrayTag", 5, "a String array tag 5 strings"));


            CidConnector connector = new CidConnector();

            bool doExport = false;

            connector.Run(tags, doExport);

        }
    }
}
