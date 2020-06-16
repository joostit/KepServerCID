// **************************************************************************
// File:  interface.cs
// Created:  11/30/2009 Copyright (c) 2009
//
// Description:  Based on command line argument, this will initiate configuration
// export or initialize and start the shared memory server.
//
// At least one device entry must be defined in the DeviceTable array. Tags
// are usually defined at compile time. However, devices may be created with
// no tags for applications that will dynamically create tags at runtime.
//
// Shared memory is configured at startup only for devices with defined tags.
// An application that creates tags dynamically would have to release, close,
// and regenerate shared memory. Any application with a reference to shared
// memory, such as KEPServer runtime, would need to stop and release the reference
// before shared memory could be closed.
//
// **************************************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;


using WORD = System.UInt16;
using DWORD = System.UInt32;
using KepServer.CidLib;
using KepServer.CidLib.Types;
using KepServer.CidLib.Tags;

namespace KepServer.CidLib.Internals
{

    // This application uses a pointer to unmanaged (shared) memory.
    // You must enable "Allow unsafe code" in project build properties.
    internal unsafe class MemInterface
    {


        // Define the devices that we will use.
        // For test purposes, you may comment out a device you do not wish to create.
        private List<DeviceEntry> deviceTable = new List<DeviceEntry>();

        // List of devices
        private List<Device> deviceSet = new List<Device>();
        private int nextDeviceIndex;      // Next device to provide to GetNextTag

        // Tag list for each device
        private List<TagEntry>[] tagEntryList;

        // shared memory class and related stream
        private SharedMemServer sharedMemoryServer = new SharedMemServer();
        public UnmanagedMemoryStream memStream;

        // CSharp mutex handling
        // Create a security object that grants no access.
        private MutexSecurity mutexSecurity = new MutexSecurity();

        private Mutex mutex = null;

        private DWORD sharedMemorySize;
        private int maxSharedMemSize = SharedMemServer.MAPSIZE;
        public bool exitFlag = false;


        public TagsCollection Tags { get; private set; }


        public MemInterface(TagsCollection tagsInfo)
        {
            this.Tags = tagsInfo;
        }

        public void Start(string strConfigName, string strApplicationDir, bool exportConfig)
        {

            LoadTagsInfo();

            //Set up a mutex to control access to shared memory
            SetupMutex(strConfigName);

            // Open the Shared Memory File with a name 
            sharedMemoryServer.Open(strConfigName);

            // Load the TAGENTRY lists for each device.
            // A device may have no tags defined at startup.
            // At startup, shared memory is configured only for defined tags.
            //LoadTagEntryLists();

            // Load the device and tag tables into lists to initialize shared memory
            LoadTables();

            // Are we exporting the configuration or running a shared memory server?
            if (exportConfig == true)
            {
                StartExportConfiguration(strApplicationDir, strConfigName);
            }
            else
            {
                // Enter the main processing loop. Return when user signals quit.
                mainLoop();
            }
            // User signaled quit
            // Close the stream.
            memStream.Close();

            //release shared mem
            if (!sharedMemoryServer.Root.Equals(null))
            {
                sharedMemoryServer.Dispose();
            }

        }

        private void LoadTagsInfo()
        {

            tagEntryList = new List<TagEntry>[Tags.Devices.Count];
            deviceTable = new List<DeviceEntry>();

            int deviceIndex = 0;
            foreach (DeviceDefinition deviceDef in Tags.Devices.Values)
            {
                DeviceEntry device = new DeviceEntry(deviceDef.Name, deviceDef.Id);
                deviceTable.Add(device);
                
                List<TagEntry> deviceTags = new List<TagEntry>();

                foreach(TagApiBase tagDef in deviceDef.Tags.Values)
                {
                    deviceTags.Add(tagDef.ToTagEntry());
                }

                tagEntryList[deviceIndex] = deviceTags;

                device.tagEntryList = deviceTags;
                deviceIndex++;
            }
        }

        public void LoadTagEntryLists()
        {
            int deviceNum;
            //match the tags to the devices in your device table

            tagEntryList = new List<TagEntry>[deviceTable.Count()];

            deviceNum = 0; // device 1 (zero-based)
            if (deviceNum >= 0 && deviceNum < deviceTable.Count())
            {

                //always instantiate the TAGENTRY list
                tagEntryList[deviceNum] = new List<TagEntry>();

                // For test purposes, you may comment out all tags you do not wish to create
                //												Name,			StringSize,	ArrayRows,	ArrayCols,          Datatype,		                ReadWrite,			         Description,			    Group
                tagEntryList[deviceNum].Add(new TagEntry("BoolTag", 0, 0, 0, ValueTypes.T_BOOL, AccessType.READWRITE, "Example boolean tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("CharTag", 0, 0, 0, ValueTypes.T_CHAR, AccessType.READWRITE, "Example signed 8 bit tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("ByteTag", 0, 0, 0, ValueTypes.T_BYTE, AccessType.READWRITE, "Example unsigned 8 bit tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("ShortTag", 0, 0, 0, ValueTypes.T_SHORT, AccessType.READWRITE, "Example signed 16-bit tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("WordTag", 0, 0, 0, ValueTypes.T_WORD, AccessType.READWRITE, "Example unsigned 16-bit tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("LongTag", 0, 0, 0, ValueTypes.T_LONG, AccessType.READWRITE, "Example signed 32-bit tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("DWordTag", 0, 0, 0, ValueTypes.T_DWORD, AccessType.READWRITE, "Example unsigned 32-bit tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("FloatTag", 0, 0, 0, ValueTypes.T_FLOAT, AccessType.READWRITE, "Example 32-bit float tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("DoubleTag", 0, 0, 0, ValueTypes.T_DOUBLE, AccessType.READWRITE, "Example double 64-bit tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("DateTag", 0, 0, 0, ValueTypes.T_DATE, AccessType.READWRITE, "Example date tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("StringTag", 15, 0, 0, ValueTypes.T_STRING, AccessType.READWRITE, "Example string tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("ReadOnlyStringTag", 15, 0, 0, ValueTypes.T_STRING, AccessType.READONLY, "Example read-only string tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("BoolArray", 0, 1, 5, ValueTypes.T_BOOL | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example bool array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("CharArray", 0, 2, 5, ValueTypes.T_CHAR | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example char array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("ByteArray", 0, 2, 5, ValueTypes.T_BYTE | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example byte array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("ShortArray", 0, 2, 5, ValueTypes.T_SHORT | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example short array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("WordArray", 0, 2, 5, ValueTypes.T_WORD | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example word array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("LongArray", 0, 2, 5, ValueTypes.T_LONG | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example long array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("DWordArray", 0, 2, 5, ValueTypes.T_DWORD | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example dword array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("FloatArray", 0, 1, 5, ValueTypes.T_FLOAT | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example float array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("DoubleArray", 0, 1, 5, ValueTypes.T_DOUBLE | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example double array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("DateArray", 0, 1, 5, ValueTypes.T_DATE | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example date array tag", ""));
                tagEntryList[deviceNum].Add(new TagEntry("StringArray", 15, 1, 5, ValueTypes.T_STRING | ValueTypes.T_ARRAY, AccessType.READWRITE, "Example string array tag", ""));

                //always assign the device TAGENTRY list
                deviceTable[deviceNum].tagEntryList = tagEntryList[deviceNum];
            }

            deviceNum = 1; // device 2 (zero-based)
            if (deviceNum >= 0 && deviceNum < deviceTable.Count())
            {

                //always instantiate the TAGENTRY list
                tagEntryList[deviceNum] = new List<TagEntry>();

                // For test purposes, you may comment out all tags you do not wish to create
                tagEntryList[deviceNum].Add(new TagEntry("FaultString", 100, 0, 0, ValueTypes.T_STRING, AccessType.READONLY, "", "X Axis\\Status"));
                tagEntryList[deviceNum].Add(new TagEntry("ServoStatus", 0, 0, 0, ValueTypes.T_DWORD, AccessType.READONLY, "", "X Axis\\Status"));
                tagEntryList[deviceNum].Add(new TagEntry("Position", 0, 0, 0, ValueTypes.T_FLOAT, AccessType.READWRITE, "", "X Axis"));
                tagEntryList[deviceNum].Add(new TagEntry("Acceleration", 0, 1, 5, ValueTypes.T_FLOAT, AccessType.READONLY, "", "X Axis"));

                //always assign the device TAGENTRY list

                deviceTable[deviceNum].tagEntryList = tagEntryList[deviceNum];
            }

        } 


        public void LoadTables()
        {
            // If Shared Memory File opened successfully, go on to define our registers
            if (sharedMemoryServer.IsOpen())
            {

                byte* sharedMemPtr = (byte*)sharedMemoryServer.Root.ToPointer();

                // Create an UnmanagedMemoryStream object using a pointer to unmanaged memory.
                memStream = new UnmanagedMemoryStream(sharedMemPtr, maxSharedMemSize, maxSharedMemSize, FileAccess.ReadWrite);

                // Important: Get the lock once.  All Shared Memory Registers will be defined while we own this lock.
                // This will eliminate the need to repeatedly lock/unlock, causing unnecessary CPU operations.
                if (mutex.WaitOne() == true)
                {
                    byte* pSharedMemoryBaseMem = sharedMemoryServer.pMem; //new mutex testing

                    // Walk device table
                    int nDeviceTableIndex = 0;
                    DWORD nextAvailableDeviceOffset = 0;
                    DWORD nextAvailableTagOffset = 0;

                    while (nDeviceTableIndex < deviceTable.Count)
                    {
                        // Create new Device
                        Device device = new Device((DeviceEntry)deviceTable[nDeviceTableIndex], this);

                        if (device.Equals(null))
                        {
                            break;
                        }

                        if (deviceTable[nDeviceTableIndex].tagEntryList.Count() == 0)
                        {
                            ++nDeviceTableIndex;
                            continue;
                        }

                        // Dynamically assign device's shared memory offset based on the previous device's allocation
                        device.SetSharedMemoryOffset(nextAvailableDeviceOffset);
                        Trace.WriteLine("Device " + device.GetName().ToString() +
                            "offset within shared memory = " + device.GetSharedMemoryOffset().ToString());

                        // Important: For this reference implementation, we will assign each device its offset within Shared Memory.
                        // This means each tag's offset must be relative to it's device's offset.
                        // In a commericial application you can choose to do it this way, or define all tag offsets relative to the 
                        // beginning of Shared Memory.  In the latter case, all device offsets would be 0.
                        nextAvailableTagOffset = 0;

                        // Add device to device set
                        deviceSet.Add(device);

                        foreach (TagEntry tagEntry in deviceTable[nDeviceTableIndex].tagEntryList)
                        {
                            string devName = deviceTable[nDeviceTableIndex].strName;
                            string tagName = tagEntry.strName;

                            TagApiBase apiTag = Tags.Devices[devName].Tags[tagName];

                            nextAvailableTagOffset = device.AddTag(tagEntry, nextAvailableTagOffset, apiTag);
                        }

                        nextAvailableDeviceOffset += nextAvailableTagOffset;
                        ++nDeviceTableIndex;
                    }
                    // Size of the map is based on the last tag allocation for the last device allocation
                    sharedMemorySize = nextAvailableDeviceOffset;

                    // Release SharedMemory
                    mutex.ReleaseMutex();
                    pSharedMemoryBaseMem = null;

                } 
                else
                {
                    System.Console.WriteLine("CRuntime.Initialize failed to acquirelock");
                }
            }

        } 


        public void mainLoop()
        {

            // **** set up and enter the main scan loop ****
            int nRC = TagData.SMRC_NO_ERROR;
            byte* pSharedMemoryBaseMem = null;
            Tag refTag = null;

            int lockCount = 0;

            while (true)
            {
                // loop til signaled to quit
                if (exitFlag == true) //the thread should set this
                    break;

                // ToDo: This mechanism and code is fubar fugly. Improve it.

                // refTag is assigned after Read/Write to "Device" and Process Read/Write Response (Shared Memory).
                // The reason we don't assign the tag now is that we would need to lock Shared Memory to check for pending requests,
                // unlock Shared Memory, read/write to "device", then lock Shared Memory again to set responses.  Locking
                // and unlocking Shared Memory twice for every tag is not desirable.  The side effect is that once a tag is assigned
                // we will need to wait one tick before we can perform the read/write.
                if (refTag != null)
                {
                    // **************************
                    // Read/Write to “Device”
                    // **************************
                    if (refTag.tagWriteRequestPending)
                    {
                        // Write means: Kepware --> CIDA
                        // Important: In a commercial application, this is where you would send the write request to the device.
                        // Since data is simulated, the write response is immediately available.
                        refTag.tagReadData.value = refTag.tagWriteData.value;       // assign value written to value to be read
                        refTag.tagReadData.quality = refTag.tagWriteData.quality;
                        refTag.tagReadData.timeStamp = refTag.tagWriteData.timeStamp;

                        refTag.tagWriteData.errorCode = 0;
                        refTag.tagWriteData.quality = TagData.OPC_QUALITY_GOOD_NON_SPECIFIC;
                        GetFtNow(ref refTag.tagWriteData.timeStamp);

                        refTag.tagWriteRequestPending = false;
                        refTag.tagWriteResponsePending = true;
                    }

                    if (refTag.tagReadRequestPending)
                    {
                        // Read means: CIDA --> Kepware
                        // Important: In a commercial application, this is where you would send the read request to the device
                        // Since data is simulated, the read response is immediately available.
                        if (refTag.IsWriteable() == true)
                        {
                            refTag.tagReadData.value.Increment();   // For simulation purposes, bump current value
                        }
                        refTag.tagReadData.errorCode = 0;
                        refTag.tagReadData.quality = TagData.OPC_QUALITY_GOOD_NON_SPECIFIC;
                        GetFtNow(ref refTag.tagReadData.timeStamp);

                        refTag.tagReadRequestPending = false;
                        refTag.tagReadResponsePending = true;
                    }

                    // ********************************************
                    // Process Read/Write Response (Shared Memory)
                    // ********************************************
                    if (refTag.tagWriteResponsePending == true || refTag.tagReadResponsePending == true)
                    {
                        Debug.Assert(pSharedMemoryBaseMem == null);

                        if (mutex.WaitOne() == true) //if we lock
                        {
                            pSharedMemoryBaseMem = sharedMemoryServer.pMem;
                        }

                        // Process responses only if we have access to Shared Memory (valid pointer)
                        if (pSharedMemoryBaseMem == null)
                        {
                            mutex.ReleaseMutex();
                            continue;
                        }

                        if (refTag.tagWriteResponsePending)
                        {

                            // Get the write response pending flag so we can ASSERT that its not set.
                            bool bWriteResponsePending = false;
                            Register.GetWriteResponsePending(memStream, refTag.GetSharedMemoryOffset(), ref bWriteResponsePending);

                            if (nRC == TagData.SMRC_NO_ERROR)
                            {
                                // CID driver should not issue a write before completing the last write
                                Debug.Assert(!bWriteResponsePending);
                                Register.SetWriteResponse(memStream, refTag.GetSharedMemoryOffset(), refTag.tagWriteData.errorCode != 0, refTag.tagWriteData.errorCode, refTag.tagWriteData.quality, refTag.tagWriteData.timeStamp);

                                if (nRC == TagData.SMRC_NO_ERROR)
                                {
                                    refTag.tagWriteResponsePending = false;
                                    refTag.NotifyNewDataAvailable();
                                }
                            }
                        } // if write response pending

                        if (refTag.tagReadResponsePending == true)
                        {
                            // Get the read response pending flag so we can ASSERT that its not set.
                            bool bReadResponsePending = false;

                            Register.GetReadResponsePending(memStream, refTag.GetSharedMemoryOffset(), ref bReadResponsePending);

                            if (nRC == TagData.SMRC_NO_ERROR)
                            {
                                // CID driver should not issue a read before completing the last read.
                                Debug.Assert(!bReadResponsePending);
                                Register.SetReadResponse(memStream, refTag.GetSharedMemoryOffset(), refTag.tagReadData.value, refTag.tagReadData.errorCode != 0, refTag.tagReadData.errorCode, refTag.tagReadData.quality, refTag.tagReadData.timeStamp);

                                if (nRC == TagData.SMRC_NO_ERROR)
                                {
                                    refTag.tagReadResponsePending = false;
                                }
                            }
                        } 
                    }
                }

                // Important: In a commercial application, you may not want to process one tag at a time as this reference implementation does.  Instead you would
                // want to process as many tags as possible, processing requests and responses.

                // Get our next tag to process.
                refTag = GetNextTag();

                if (refTag != null)
                {
                    // ********************************************
                    // Process Read/Write Requests (Shared Memory)
                    // ********************************************

                    // May have been locked above
                    if (pSharedMemoryBaseMem == null)
                    {
                        try
                        {
                            if (mutex.WaitOne() == true)
                            {
                                lockCount++;
                                pSharedMemoryBaseMem = sharedMemoryServer.pMem;
                            }
                        }
                        catch (AbandonedMutexException ex)
                        {
                            Console.WriteLine("Exception on return from WaitOne." +
                                "\r\n\tMessage: {0}", ex.Message);
                        }
                    }
                    // Process requests only if we have access to Shared Memory (valid pointer)
                    if (pSharedMemoryBaseMem == null)
                    {
                        if (lockCount > 0)
                        {
                            mutex.ReleaseMutex();
                            lockCount--;
                        }
                        continue;
                    }

                    // Check for pending write requests
                    bool WriteRequestPending = false;
                    nRC = Register.GetWriteRequestPending(memStream, refTag.GetSharedMemoryOffset(), ref WriteRequestPending);

                    if (nRC == TagData.SMRC_NO_ERROR)
                    {
                        if (WriteRequestPending)
                        {
                            nRC = Register.GetWriteRequest(memStream, refTag.GetSharedMemoryOffset(), ref refTag.tagWriteData.value, ref refTag.tagWriteData.quality, ref refTag.tagWriteData.timeStamp);

                            if (nRC == TagData.SMRC_NO_ERROR)
                            {
                                refTag.tagWriteRequestPending = true;
                            }
                        }
                    }

                    // Check for pending read requests
                    bool bReadRequestPending = false;
                    nRC = Register.GetReadRequestPending(memStream, refTag.GetSharedMemoryOffset(), ref bReadRequestPending);

                    if (nRC == TagData.SMRC_NO_ERROR)
                    {
                        if (bReadRequestPending)
                        {
                            nRC = Register.GetReadRequest(memStream, refTag.GetSharedMemoryOffset());

                            if (nRC == TagData.SMRC_NO_ERROR)
                            {
                                refTag.tagReadRequestPending = true;
                            }
                        }
                    }

                    try
                    {
                        mutex.ReleaseMutex();
                        lockCount--;
                    }
                    catch (SystemException ex)
                    {
                        Console.WriteLine("Exception on return from ReleaseMutex ()." +
                            "\r\n\tMessage: {0}", ex.Message);
                    }
                    pSharedMemoryBaseMem = null;
                }
            } 

            // The only time we should fall out of this function is if the quit event is set
            // and that only happens when we are shutting down

        }



        public void StartExportConfiguration(string strApplicationDir, string strConfigName)
        {
            // Create a configuration file in the application's directory
            string strConfigFile = strApplicationDir;
            strConfigFile += "\\" + strConfigName;
            strConfigFile += ".xml";

            FileStream sFile = null;
            bool goodStream = true;
            try
            {
                sFile = new FileStream(strConfigFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            }
            catch (SystemException ex)
            {
                Console.WriteLine("Unable to export configuration.  FileStream failed with error " + ex.Message);
                goodStream = false;
            }

            if (goodStream == true)
            {
                using (sFile)
                {
                    // Export configuration
                    // Release file
                    sFile.Close();
                    ExportConfiguration(strConfigFile, strConfigName);
                    Console.WriteLine("Created xml config file. Press a key to finish");
                    Console.ReadKey();
                }
            }

        } 



        public void ExportConfiguration(string strConfigFile, string strConfigName)
        {
            // Create the Configuration File in XML format

            // Important: For the sake of example, the reference implementation will create the XML file in a rudimentary fashion.
            // The vendor is free to utilize preferred techniques for generating XML, such as DOM.

            if (File.Exists(strConfigFile))
            {
                // Create a file to write to.
                File.AppendAllText(strConfigFile, "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:Configuration xmlns:custom_interface_config=\"http://www.kepware.com/schemas/custom_interface_config\">");
                File.AppendAllText(strConfigFile, "<custom_interface_config:Copyright>The CID Interface and CID Definition file formats are the Copyright of Kepware Technologies and may only be used with KEPServer based products.  Use of the CID interfaces and file formats in any other manner is strictly forbidden.</custom_interface_config:Copyright>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:Name>" + strConfigName + "</custom_interface_config:Name>");
                //// Support Info
                File.AppendAllText(strConfigFile, "<custom_interface_config:SupportInfo>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:ContactInfo>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:CompanyName>My Company</custom_interface_config:CompanyName>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:Phone>1-888-555-1212</custom_interface_config:Phone>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:Email>support@mycompany.com</custom_interface_config:Email>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:WebURL>www.mycompany.com</custom_interface_config:WebURL>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:ContactAdditional></custom_interface_config:ContactAdditional>");
                File.AppendAllText(strConfigFile, "</custom_interface_config:ContactInfo>\r\n");
                File.AppendAllText(strConfigFile, "<custom_interface_config:ConfigurationLaunchHint>To export configuration, use argument -exportconfig.  Example: cidarefimplcpp.exe -exportconfig</custom_interface_config:ConfigurationLaunchHint>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:RuntimeLaunchHint>Run without any arguments.  Example: cidarefimplcpp.exe</custom_interface_config:RuntimeLaunchHint>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:HelpLaunchHint></custom_interface_config:HelpLaunchHint>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:SupportAdditional></custom_interface_config:SupportAdditional>");
                File.AppendAllText(strConfigFile, "</custom_interface_config:SupportInfo>");
            }

            if (deviceSet.Count > 0)
            {
                // Walk the device set
                // Shared Memory Size
                File.AppendAllText(strConfigFile, "<custom_interface_config:SharedMemorySize>" + sharedMemorySize + "</custom_interface_config:SharedMemorySize>");
                // <device list>
                File.AppendAllText(strConfigFile, "<custom_interface_config:DeviceList>");
                // Have each device export its configuration
                foreach (Device device in deviceSet)
                {
                    // Call on device to export its configuration
                    device.ExportConfiguration(strConfigFile);
                }
                File.AppendAllText(strConfigFile, "</custom_interface_config:DeviceList>");
            }
            File.AppendAllText(strConfigFile, "</custom_interface_config:Configuration>");
        }



        public void SetupMutex(string strConfigName)
        {
            //Set up a mutex to control access to shared memory
            // The value of this variable is set by the mutex
            // constructor. It is true if the named system mutex was
            // created, and false if the named mutex already existed.
            //
            bool createdNewMutex = false;

            //string strConfigName = "cidarefimplcsharp";
            // We need to have a name
            string strName = strConfigName;
            Debug.Assert(!strName.Equals(""));

            // Name should be prefixed with 'Global\\' for Vista support
            if (strName.Substring(0, 7) != "Global\\")
            {
                strName = "Global\\" + strName;
            }

            strName += "_sm_lock";

            MutexAccessRule rule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.Synchronize | MutexRights.Modify |
                MutexRights.ReadPermissions | MutexRights.FullControl |
                MutexRights.TakeOwnership,
                AccessControlType.Allow);

            mutexSecurity.AddAccessRule(rule);

            // Rewrote the Mutex constructor call for migration from .NET Framework 3.5 to .NET Core 3.2
            //mutex = new Mutex (true, strName, out createdNewMutex, mSec);
            mutex = new Mutex(true, strName, out createdNewMutex);
            mutex.SetAccessControl(mutexSecurity);



            if (createdNewMutex == true)
            {
                //System.Console.WriteLine ("Success: created mutex {0}", strName);
                mutex.ReleaseMutex();//that was requested at creation
            }
            else
            {
                //System.Console.WriteLine ("Failed to create mutex {0}", strName);
            }

        } 


        // **************************************************************************
        // GetNextTag
        // Purpose: Provide caller with the next tag to work with.  Next tag is based
        // on order of the tag set within each device set, which is also ordered.
        // **************************************************************************
        public Tag GetNextTag()
        {

            // Look for empty set
            if (deviceSet.Count == 0)
            {
                return (null);
            }

            // Upon rollover, start from beginning
            if (deviceSet.Count == 1)
            {
                nextDeviceIndex = 0;
            }
            else if (nextDeviceIndex > deviceSet.Count - 1) // if at the last index
            {
                nextDeviceIndex = 0;
                //also reset the tag iterator for this device for the next pass
                deviceSet[nextDeviceIndex].ResetTagIterator();
            }

            Device refDevice = deviceSet[nextDeviceIndex];

            bool bIsLast = false;

            Tag refTag = null;

            if (refDevice != null)
            {
                refTag = Device.GetNextTag(ref refDevice, ref bIsLast);

                // If refTag is last tag current device's list, move to next device
                if (bIsLast)
                    nextDeviceIndex++;
            }

            return (refTag);

        } 


        public void GetFtNow(ref FileTime ft)
        {
            long hFT1 = DateTime.Now.ToFileTime();
            ft.dwLowDateTime = (UInt32)(hFT1 & 0xFFFFFFFF);
            ft.dwHighDateTime = (UInt32)(hFT1 >> 32);
        }

    } 

} 
