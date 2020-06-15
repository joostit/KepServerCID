// **************************************************************************
// File:  device.cs
// Created:  11/30/2009 Copyright (c) 2009
//
// Description: Defines the Device class.  The Device class includes the
// name of the device, its device id, its offset within Shared Memory,
// and the Tags that belong to it.  In addition the Device is responsible
// for exporting its definition to the Configuration file when requested.
//
// **************************************************************************
#define TRACE_SM_ACCESS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using WORD = System.UInt16;
using DWORD = System.UInt32;
using KepServer.CidLib.Types;

namespace CidaRefImplCsharp
{
    public class Device
    {
        // Properties from table
        public string devName;
        public string devID;

        private MemInterface memInterface;

        // Properties calculated
        public DWORD devSharedMemoryOffset;

        public List<Tag> tagSet;
        public static int nextTagIndex;		// Next tag to provide to GetNextTag

        // *************************************************************************************
        public Device(DeviceEntry tDeviceEntry, MemInterface memInterface)
        {
            this.memInterface = memInterface;
            devName = tDeviceEntry.strName;
            devID = tDeviceEntry.strID;

            devSharedMemoryOffset = 0;

            tagSet = new List<Tag>();
            nextTagIndex = tagSet.Count; //this will be zero, and the starting index
        }

        // *************************************************************************************
        public void ExportConfiguration(string strConfigFile)
        {
            if (File.Exists(strConfigFile))
            {
                File.AppendAllText(strConfigFile, "<custom_interface_config:Device>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:Name>" + devName + "</custom_interface_config:Name>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:ID>" + devID + "</custom_interface_config:ID>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:SharedMemoryDeviceOffset>" + devSharedMemoryOffset + "</custom_interface_config:SharedMemoryDeviceOffset>");

                //// Walk the tag set for this device
                File.AppendAllText(strConfigFile, "<custom_interface_config:TagList>");

                // Have each tag export its configuration
                foreach (Tag refTag in tagSet)
                {
                    //System.Console.WriteLine ("refTag.tagRelativeOffset = {0}  {1}", refTag.tagRelativeOffset, refTag.tagDataType);
                    refTag.ExportConfiguration(strConfigFile);
                }

                File.AppendAllText(strConfigFile, "</custom_interface_config:TagList>");
                File.AppendAllText(strConfigFile, "</custom_interface_config:Device>");
            }
        } // ExportConfiguration (string strConfigFile)

        // *************************************************************************************
        // Name
        public string GetName()
        {
            return (devName);
        }

        // *************************************************************************************
        // ID
        public string GetID()
        {
            return (devID);
        }

        // *************************************************************************************
        // Shared Memory Offset
        public void SetSharedMemoryOffset(DWORD sharedMemoryOffset)
        {
            devSharedMemoryOffset = sharedMemoryOffset;
        }

        // *************************************************************************************
        public DWORD GetSharedMemoryOffset()
        {
            return (devSharedMemoryOffset);
        }

        // *************************************************************************************
        public void ResetTagIterator()
        {
            nextTagIndex = 0;
        }

        //**************************************************************
        public DWORD AddTag(TagEntry tTagEntry, DWORD relativeOffset)
        {
            DWORD readOffset = 0;
            DWORD writeOffset = 0;
            DWORD nextAvailableDataOffset = 0;	// offset for next data block
            DWORD nextAvailableTagOffset = 0;     // offset for next tag

            Tag refTag; //= NULL;

            Device refDevice = this;
            // Create new Tag
            refTag = new Tag(memInterface, ref refDevice, tTagEntry, relativeOffset, GetSharedMemoryOffset());

            if (refTag.Equals(null))
            {
                Trace.Assert(false);
                return (relativeOffset);    // re-use this offset for the next tag
            }

            // Determine register's read offset
            if (refTag.IsReadable())
            {
                readOffset = Register.DATA_MIN_OFFSET;
                nextAvailableDataOffset = readOffset + Register.DATA_SIZE_MIN + refTag.GetReadValueExtSize();

                // Determine register's write offset
                if (refTag.IsWriteable())
                {
                    // read/write
                    writeOffset = nextAvailableDataOffset;
                    nextAvailableDataOffset = writeOffset + Register.DATA_SIZE_MIN + refTag.GetWriteValueExtSize();
                }
                else
                {
                    // read-only
                    writeOffset = 0;
                }

            }
            else
            {
                readOffset = 0;

                // Determine register's write offset
                if (refTag.IsWriteable())
                {
                    // write-only
                    writeOffset = Register.DATA_MIN_OFFSET;
                    nextAvailableDataOffset = writeOffset + Register.DATA_SIZE_MIN + refTag.GetWriteValueExtSize();
                }
                else
                {
                    // neither read or write
                    Debug.Assert(false);
                    writeOffset = 0;
                }
            }

            // Prepare the register offset for the next tag (if applicable)
            nextAvailableTagOffset = relativeOffset + nextAvailableDataOffset;

            string msg = "";

            // Check for Shared Memory file overrun
            if ((GetSharedMemoryOffset() + nextAvailableTagOffset) > SharedMemServer.MAPSIZE)
            {
                // Reached the limits of our Shared Memory file
                // Don't add tag

                msg = string.Format("Tag {0,0:T} could not be added.", refTag.GetName()) +
                    string.Format("Register {0,0:D} with size ", refTag.GetSharedMemoryOffset()) +
                    string.Format("{0,0:D} bytes would exceed Shared Memory file size of ", (DWORD)(writeOffset + Register.DATA_SIZE_MIN + refTag.GetExtendedSize())) +
                    string.Format("{0,0:D}", SharedMemServer.MAPSIZE);
                Trace.WriteLine(msg);

                return (relativeOffset);
            }

            int nRC = TagData.SMRC_NO_ERROR;

            // Initialize register if we have access to shared memory
            if (memInterface.memStream != null)
            {
                Register.SetReadOffset(memInterface.memStream, (long)refTag.GetSharedMemoryOffset(), readOffset);

#if TRACE_SM_ACCESS
                msg = string.Format("{0,8:0D}: Tag " +
                    "{1,0:T}" + ": SetReadOffset nRC = " +
                    "{2,0:D}" +
                    ", dwReadOffset = {3,0:D}", GetTickCount(), refTag.GetName(), nRC, readOffset);
                Trace.WriteLine(msg);
#endif//TRACE_SM_ACCESS

                Register.SetWriteOffset(memInterface.memStream, (long)refTag.GetSharedMemoryOffset(), writeOffset);

#if TRACE_SM_ACCESS
                msg = string.Format("{0,8:0D}: Tag " +
                    "{1,0:T}: SetWriteOffset nRC = " +
                    "{2,0:D}" +
                    ", dwWriteOffset = {3,0:D}", GetTickCount(), refTag.GetName(), nRC, writeOffset);
                Trace.WriteLine(msg);
#endif//TRACE_SM_ACCESS

                Register.SetReadValueType(memInterface.memStream, (long)refTag.GetSharedMemoryOffset(), refTag.GetReadValueType());

#if TRACE_SM_ACCESS
                msg = string.Format("{0,8:0D}: Tag " +
                    "{1,0:T}: SetReadValueType nRC = " +
                    "{2,0:D}" +
                    ", ReadValueType = {3,0:D}", GetTickCount(), refTag.GetName(), nRC, refTag.GetReadValueType());
                Trace.WriteLine(msg);
#endif//TRACE_SM_ACCESS

                if (writeOffset != 0)
                {
                    Register.SetWriteValueType(memInterface.memStream, (long)refTag.GetSharedMemoryOffset(), refTag.GetWriteValueType());

#if TRACE_SM_ACCESS
                    msg = string.Format("{0,8:0D}: Tag " +
                        "{1,0:T}: SetWriteValueType nRC = " +
                        "{2,0:D}" +
                        ", WriteValueType = {3,0:D}", GetTickCount(), refTag.GetName(), nRC, refTag.GetWriteValueType());
                    Trace.WriteLine(msg);
#endif//TRACE_SM_ACCESS
                }
                Register.SetReadValueExtSize(memInterface.memStream, (long)refTag.GetSharedMemoryOffset(), refTag.GetReadValueExtSize());

#if TRACE_SM_ACCESS
                msg = string.Format("{0,8:0D}: Tag " +
                    "{1,0:T}: SetReadValueExtSize nRC = " +
                    "{2,0:D}" +
                    ", ReadValueExtSize = {3,0:D}", GetTickCount(), refTag.GetName(), nRC, refTag.GetReadValueExtSize());
                Trace.WriteLine(msg);
#endif//TRACE_SM_ACCESS
                if (writeOffset != 0)
                {
                    Register.SetWriteValueExtSize(memInterface.memStream, (long)refTag.GetSharedMemoryOffset(), refTag.GetWriteValueExtSize());

#if TRACE_SM_ACCESS
                    msg = string.Format("{0,8:0D}: Tag " +
                        "{1,0:T}: SetWriteValueExtSize nRC = " +
                        "{2,0:D}" +
                        ", WriteValueExtSize = {3,0:D}", GetTickCount(), refTag.GetName(), nRC, refTag.GetWriteValueExtSize());
                    Trace.WriteLine(msg);
#endif//TRACE_SM_ACCESS
                }

                if (refTag.GetDataType() == (ushort)(Value.T_ARRAY | Value.T_STRING))
                {
                    nRC = Register.SetReadValueArrayStringSize(memInterface.memStream, (long)refTag.GetSharedMemoryOffset(), refTag.GetReadValueArrayStringSize());

#if TRACE_SM_ACCESS
                    msg = string.Format("{0,8:0D}: Tag " +
                        "{1,0:T}: SetReadValueArrayStringSize nRC = " +
                        "{2,0:D}" +
                        ", ReadValueArrayStringSize = {3,0:D}", GetTickCount(), refTag.GetName(), nRC, refTag.GetReadValueArrayStringSize());
                    Trace.WriteLine(msg);
#endif//TRACE_SM_ACCESS
                    if (writeOffset != 0)
                    {
                        nRC = Register.SetWriteValueArrayStringSize(memInterface.memStream, (long)refTag.GetSharedMemoryOffset(), (WORD)refTag.GetWriteValueArrayStringSize());

#if TRACE_SM_ACCESS
                        msg = string.Format("{0,8:0D}: Tag " +
                            "{1,0:T}: SetWriteValueArrayStringSize nRC = " +
                            "{2,0:D}" +
                            ", WriteValueArrayStringSize = {3,0:D}", GetTickCount(), refTag.GetName(), nRC, refTag.GetWriteValueArrayStringSize());
                        Trace.WriteLine(msg);
#endif//TRACE_SM_ACCESS
                    }
                }

            } // if (pSharedMemoryBaseMem != null)

            // Add tag to our tag set
            tagSet.Add(refTag);
            msg = string.Format("Tag {0,0:T}: assigned to Register " +
                "{1,0:D} with size " +
                "{2,0:D} bytes.  Relative offset = " +
                "{3,0:D}", refTag.GetName(), refTag.GetSharedMemoryOffset(),
                    nextAvailableDataOffset,
                    refTag.GetRelativeOffset());
            Trace.WriteLine(msg);

            return (nextAvailableTagOffset);
        } // AddTag ()


        // *************************************************************************************
        public static Tag GetNextTag(ref Device pDev, ref bool bIsLast)
        {
            bIsLast = false;

            // Look for empty set
            if (pDev.tagSet.Count == 0)
            {
                return (null);
            }
            Tag retTag = pDev.tagSet[nextTagIndex];

            // If this is the last tag in the list, set flag
            if (++nextTagIndex == pDev.tagSet.Count)
            {
                bIsLast = true;
                nextTagIndex = 0;
            }

            return (retTag);
        } //  GetNextTag (ref Device pDev, ref bool bIsLast)

        // *************************************************************************************
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetTickCount();

     
    } // class Device

} // namespace CidaRefImplCsharp
