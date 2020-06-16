// **************************************************************************
// File:  appmain.cs
// Created:  12/03/2009 Copyright (c) 2009
//
// Description: Main entry point. Creates a configuration name based on the
// the application name. Calls the MemInterface class to initialize shared memory
// or initiate configuration export based on command line argument.
//
// **************************************************************************
// class library references
using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KepServer.CidLib
{


    // This application uses a pointer to unmanaged (shared) memory.
    // To build, you must enable "Allow unsafe code" in project build properties.
    public class CidConnector
    {

        public TagsCollection Tags
        {
            get
            {
                return memInterface.Tags;
            }
        }

        private MemInterface memInterface;

        private Thread cidRunnerThread;

        public void Start(TagsCollection tagsInfo)
        {

            string strConfigName = "";
            string strApplicationDir = "";

            cidRunnerThread = new Thread((o) =>
            {
                // Get application info for naming the shared memory file and mutex
                GetConfigInfo(ref strConfigName, ref strApplicationDir);

                // Start the interface to shared memory
                memInterface = new MemInterface(tagsInfo);
                memInterface.Start(strConfigName, strApplicationDir, false);
            });

            cidRunnerThread.Name = "CID_Runner";
            cidRunnerThread.IsBackground = true;
            cidRunnerThread.Start();
        }

        public void Stop()
        {
            memInterface.exitFlag = true;
            cidRunnerThread.Join();
        }


        public void ExportConfiguration(TagsCollection tagsInfo)
        {
            string strConfigName = "";
            string strApplicationDir = "";

            // Get application info for naming the shared memory file and mutex
            GetConfigInfo(ref strConfigName, ref strApplicationDir);

            // Start the interface to shared memory
            memInterface = new MemInterface(tagsInfo);
            memInterface.Start(strConfigName, strApplicationDir, true);
        }


        private void GetConfigInfo(ref string strConfigName, ref string strApplicationDir)
        {
            // Important: The following will generate a Configuration Name based on the application's binary file name.
            // This is not a requirement and can be anything the vendor desires.

            // Get the application's directory
            string strTempApplicationDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            // Trim the leading chars 'file:\'
            char[] TrimChar = { 'f', 'i', 'l', 'e', ':', '\\' };
            strApplicationDir = strTempApplicationDir.TrimStart(TrimChar);

            // Get the application's name without the .exe extension
            strConfigName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        } 

    } 

}
