using KepServer.CidLib.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.Cid.CidTester
{
    class FakeDeviceTags
    {
        public TagsCollection Tags { get; private set; }

        public WordTag WordTag { get; private set; }
        public BoolTag BoolTag { get; private set; }
        public CharTag CharTag { get; private set; }
        public ByteTag ByteTag { get; set; }
        public ShortTag ShortTag { get; set; }
        public LongTag LongTag { get; set; }
        public DWordTag DWordTag { get; set; }
        public FloatTag FloatTag { get; set; }
        public DoubleTag DoubleTag { get; set; }
        public DateTag DateTag { get; set; }
        public StringTag StringTag { get; set; }
        public BoolArrayTag BoolArrayTag { get; set; }
        public CharArrayTag CharArrayTag { get; set; }
        public ByteArrayTag ByteArrayTag { get; set; }
        public ShortArrayTag ShortArrayTag { get; set; }
        public WordArrayTag WordArrayTag { get; set; }
        public LongArrayTag LongArrayTag { get; set; }
        public DWordArrayTag DWordArrayTag { get; set; }
        public FloatArrayTag FloatArrayTag { get; set; }
        public DoubleArrayTag DoubleArrayTag { get; set; }
        public DateArrayTag DateArrayTag { get; set; }
        public StringArrayTag StringArrayTag { get; set; }

        public FakeDeviceTags()
        {
            CreateTags();
            HookUpDataEvents();
        }

        private void CreateTags()
        {
            Tags = new TagsCollection();
            
            Tags.AddDevice("MyTestDevice", "DeviceIdblabla")
                .AddTag(WordTag = new WordTag("aWordTag", "Some fake tag"))
                .AddTag(BoolTag = new BoolTag("aBoolTag", "a Boolean tag"))
                .AddTag(CharTag = new CharTag("aCharTag", "a Character tag"))
                .AddTag(ByteTag = new ByteTag("aByteTag", "a Byte tag"))
                .AddTag(ShortTag = new ShortTag("aShortTag", "a Short tag"))
                .AddTag(LongTag = new LongTag("aLongTag", "a long tag"))
                .AddTag(DWordTag = new DWordTag("aDWordTag", "a DWORD tag"))
                .AddTag(FloatTag = new FloatTag("aFloatTag", "a Float tag"))
                .AddTag(DoubleTag = new DoubleTag("aDoubleTag", "a Double tag"))
                .AddTag(DateTag = new DateTag("aDateTag", "a Date tag"))
                .AddTag(StringTag = new StringTag("aStringTag", "a String tag", 16))
                .AddTag(BoolArrayTag = new BoolArrayTag("aBoolArrayTag", 3, 4, "a Boolean array tag of 3 x 4"))
                .AddTag(CharArrayTag = new CharArrayTag("aCharArrayTag", 3, 4, "a Char array tag of 3 x 4"))
                .AddTag(ByteArrayTag = new ByteArrayTag("aByteArrayTag", 3, 4, "a Byte array tag of 3 x 4"))
                .AddTag(ShortArrayTag = new ShortArrayTag("aShortArrayTag", 3, 4, "a Short array tag of 3 x 4"))
                .AddTag(WordArrayTag = new WordArrayTag("aWordArrayTag", 3, 4, "a Word array tag of 3 x 4"))
                .AddTag(LongArrayTag = new LongArrayTag("aLongArrayTag", 3, 4, "a Long array tag of 3 x 4"))
                .AddTag(DWordArrayTag = new DWordArrayTag("aDWordArrayTag", 3, 4, "a DWORD array tag of 3 x 4"))
                .AddTag(FloatArrayTag = new FloatArrayTag("aFloatArrayTag", 3, 4, "a Float array tag of 3 x 4"))
                .AddTag(DoubleArrayTag = new DoubleArrayTag("aDoubleArrayTag", 3, 4, "a Double array tag of 3 x 4"))
                .AddTag(DateArrayTag = new DateArrayTag("aDateArrayTag", 3, 4, "a Date array tag of 3 x 4"))
                .AddTag(StringArrayTag = new StringArrayTag("aStringArrayTag", 5, "a String array tag 5 strings"));
        }


        private void HookUpDataEvents()
        {
            WordTag.NewDataAvailable += (s, e) => Console.WriteLine($"WordTag: New data written: {WordTag.Value}");
            BoolTag.NewDataAvailable += (s, e) => Console.WriteLine($"BoolTag: New data written: {BoolTag.Value}");
            CharTag.NewDataAvailable += (s, e) => Console.WriteLine($"CharTag: New data written: {CharTag.Value}");
            ByteTag.NewDataAvailable += (s, e) => Console.WriteLine($"ByteTag: New data written: {ByteTag.Value}");
            ShortTag.NewDataAvailable += (s, e) => Console.WriteLine($"ShortTag: New data written: {ShortTag.Value}");
            LongTag.NewDataAvailable += (s, e) => Console.WriteLine($"LongTag: New data written: {LongTag.Value}");
            DWordTag.NewDataAvailable += (s, e) => Console.WriteLine($"DWordTag: New data written: {DWordTag.Value}");
            FloatTag.NewDataAvailable += (s, e) => Console.WriteLine($"FloatTag: New data written: {FloatTag.Value}");
            DoubleTag.NewDataAvailable += (s, e) => Console.WriteLine($"DoubleTag: New data written: {DoubleTag.Value}");
            DateTag.NewDataAvailable += (s, e) => Console.WriteLine($"DateTag: New data written: {DateTag.Value}");
            StringTag.NewDataAvailable += (s, e) => Console.WriteLine($"StringTag: New data written: {StringTag.Value}");

            BoolArrayTag.NewDataAvailable += (s, e) =>
            {
                Console.WriteLine("BoolArrayTag: New data written");
                PrintArray(BoolArrayTag.Value);
                Console.WriteLine(BoolArrayTag[0, 1].ToString());
            };

        }


        private void PrintArray(Array toPrint)
        {
            StringBuilder sb = new StringBuilder();
            for (int r = 0; r <= toPrint.GetUpperBound(0); r++)
            {
                for (int c = 0; c <= toPrint.GetUpperBound(1); c++)
                {
                    sb.Append(toPrint.GetValue(r, c).ToString());
                    sb.Append(" ");
                }
                sb.Append("\n");
            }
            Console.WriteLine(sb.ToString());
        }

    }
}
