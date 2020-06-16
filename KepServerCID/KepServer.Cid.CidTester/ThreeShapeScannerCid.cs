using KepServer.CidLib.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.Cid.CidTester
{
    class ThreeShapeScannerCid
    {
        public TagsCollection Tags { get; private set; }

        public BoolTag IsRunning { get; private set; }
        public WordTag JobCount { get; private set; }
        public StringTag JobSource { get; private set; }

        public ThreeShapeScannerCid()
        {

            CreateTags();
            HookUpEvents();
        }


        private void CreateTags()
        {
            Tags = new TagsCollection();

            Tags.AddDevice("TestScanner", "A scanner for testing")
                .AddTag(IsRunning = new BoolTag("IsRunning", "Indicates whether the scanner is running"))
                .AddTag(JobCount = new WordTag("JobCount", "The total number of jobs done"))
                .AddTag(JobSource = new StringTag("JobSource", "Sets the directory for the scanner to take its jobs", 64));
        }


        private void HookUpEvents()
        {
            JobSource.NewDataAvailable += (o, e) => Console.WriteLine($"Sending new Job source to the scanner... {JobSource.Value}");
        }
    }
}
