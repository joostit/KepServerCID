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


    }
}
