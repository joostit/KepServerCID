using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

using WORD = System.UInt16;
using DWORD = System.UInt32;
using VALTYPE = System.UInt16;
using BOOL = System.UInt16;
using KepServer.CidLib.Types;

namespace KepServer.CidLib.Internals
{
    /// <summary>
    /// Description:  Defines the Tag class.  The Tag class includes the
    /// name of the tag, its datatype, and its offset within Shared Memory to name
    /// a few.  It also maintains a read data class and write data class.
    /// These data classes are separate from the Shared Memory DATA byte array and
    /// are meant to store locally, the value from a "read response" and the value
    /// for a "write request".  In a commerical application these values would be
    /// used in communicating with an actual device. The Device is responsible
    /// for exporting its definition to the Configuration file when requested.
    /// </summary>
    public class Tag
    {
        // Properties from table
        public string tagName;
        public VALTYPE tagDataType;
        public AccessType tagReadWrite;
        public int tagScanRateMS;
        public string tagDescription;
        public string tagGroupName;
        public WORD tagStringSize;
        public WORD tagArrayRows;
        public WORD tagArrayCols;

        // Properties calculated
        private DWORD tagRelativeOffset;			// Register's offset relative to device's shared memory offset
        private DWORD tagSharedMemoryOffset;		// Register's absolute offset in shared memory

        internal bool tagWriteRequestPending;
        internal bool tagReadRequestPending;
        internal bool tagWriteResponsePending;
        internal bool tagReadResponsePending;

        public Device tagDevice; //reference to the device that owns this tag

        public TagData tagReadData;
        public TagData tagWriteData;

        public event EventHandler NewDataAvailable;

        public string GetName()
        {
            return (tagName);
        }


        internal Tag(MemInterface memInterface, ref Device device, TagEntry tTagEntry, DWORD dwRelativeOffset, DWORD dwParentOffset)
        {
            tagDevice = device;

            tagName = tTagEntry.strName;
            tagStringSize = tTagEntry.stringSize;
            tagArrayRows = tTagEntry.arrayRows;
            tagArrayCols = tTagEntry.arrayCols;
            tagDataType = tTagEntry.dataType;
            tagReadWrite = tTagEntry.Access;
            tagScanRateMS = 100;
            tagDescription = tTagEntry.description;
            tagGroupName = tTagEntry.groupName;

            tagRelativeOffset = dwRelativeOffset;
            tagSharedMemoryOffset = dwParentOffset + dwRelativeOffset;
            tagWriteRequestPending = false;
            tagReadRequestPending = false;
            tagWriteResponsePending = false;
            tagReadResponsePending = false;

            if ((tTagEntry.Access == AccessType.READONLY) ||
                (tTagEntry.Access == AccessType.READWRITE))
            {
                tagReadData = new TagData(memInterface, tTagEntry.dataType,
                    tTagEntry.stringSize, tTagEntry.arrayRows, tTagEntry.arrayCols);
            }

            if ((tTagEntry.Access == AccessType.READWRITE) ||
                (tTagEntry.Access == AccessType.WRITEONLY))
            {
                tagWriteData = new TagData(memInterface, tTagEntry.dataType,
                    tTagEntry.stringSize, tTagEntry.arrayRows, tTagEntry.arrayCols);
            }

        }


        internal void NotifyNewDataAvailable()
        {
            NewDataAvailable?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Purpose: Format an address to meet the expectation of the CID driver (e.g. D0)
        /// This is not to be confused with a PLC address.
        /// </summary>
        /// <param name="strAddress"></param>
        public void GetCIDAddress(ref string strAddress)
        {
            // Format register number
            string strBuffer = string.Format("D{0,0:D}", tagRelativeOffset);
            if (strBuffer.Length > 16)
                strBuffer = strBuffer.Substring(0, 16);
            strAddress = strBuffer;

            string strExtBuffer = "";

            if (tagDataType == ValueTypes.T_STRING)
            {
                // Format \string length
                strExtBuffer = string.Format("/{0,0:D}", tagStringSize);
                if (strExtBuffer.Length > 32)
                    strExtBuffer = strExtBuffer.Substring(0, 32);
                strAddress += strExtBuffer;
            }

            else if (IsArrayType(tagDataType) && (tagArrayRows != 0) && (tagArrayCols != 0))
            {
                if (tagArrayRows == 1)
                {
                    // Format [cols]
                    if ((tagDataType & ~ValueTypes.T_ARRAY) == ValueTypes.T_STRING)
                    {
                        strExtBuffer = string.Format("/{0,0:D} [{1,0:D}] ", tagStringSize, tagArrayCols);
                        if (strExtBuffer.Length > 32)
                        {
                            strExtBuffer = strExtBuffer.Substring(0, 32);
                        }
                    }

                    else
                    {
                        strExtBuffer = string.Format("[{0,0:D}] ", tagArrayCols);
                    }
                    if (strExtBuffer.Length > 32)
                    {
                        strExtBuffer = strExtBuffer.Substring(0, 32);
                    }

                }
                else
                {
                    // Format [rows][cols]
                    Debug.Assert((tagDataType & ~ValueTypes.T_ARRAY) != ValueTypes.T_STRING);	// // Multi-dimensional string arrays are not allowed
                    strExtBuffer = string.Format("[{0,0:D}] [{1,0:D}]", tagArrayRows, tagArrayCols);
                    if (strExtBuffer.Length > 32)
                    {
                        strExtBuffer = strExtBuffer.Substring(0, 32);
                    }
                }
                strAddress += strExtBuffer;
            }
        }



        public void ExportConfiguration(string strConfigFile)
        {
            // Generate address to be used by CID
            string strAddress = "";
            GetCIDAddress(ref strAddress);

            // Generate datatype name to be used by CID
            VALTYPE dataType = tagDataType;

            string strDataType;
            strDataType = TextFromType(dataType);

            // Generate read/write string to be used by CID
            string strReadWrite = "";
            GetReadWriteAsText(ref strReadWrite);

            if (File.Exists(strConfigFile))
            {
                File.AppendAllText(strConfigFile, "<custom_interface_config:Tag>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:Name>" + tagName + "</custom_interface_config:Name>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:Address>" + strAddress + "</custom_interface_config:Address>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:DataType>" + strDataType + "</custom_interface_config:DataType>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:ReadWriteAccess>" + strReadWrite + "</custom_interface_config:ReadWriteAccess>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:ScanRateMilliseconds>" + tagScanRateMS + "</custom_interface_config:ScanRateMilliseconds>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:Description>" + tagDescription + "</custom_interface_config:Description>");
                File.AppendAllText(strConfigFile, "<custom_interface_config:GroupName>" + tagGroupName + "</custom_interface_config:GroupName>");
                File.AppendAllText(strConfigFile, "</custom_interface_config:Tag>");
            }
        }


        /// <summary>
        /// This is how we make an array type from a basic type.
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static VALTYPE MakeArrayType(VALTYPE dataType)
        {
            return (VALTYPE)(dataType | ValueTypes.T_ARRAY);
        }


        public static VALTYPE MakeBasicType(VALTYPE dataType)
        {
            return (VALTYPE)(dataType & ~ValueTypes.T_ARRAY);
        }


        public static bool IsArrayType(VALTYPE dataType)
        {
            return ((dataType & ValueTypes.T_ARRAY) == ValueTypes.T_ARRAY ? true : false);
        }


        public static string TextFromType(VALTYPE valueType)
        {
            switch (valueType)
            {
                case ValueTypes.T_BOOL: return ("Boolean");
                case ValueTypes.T_BYTE: return ("Byte");
                case ValueTypes.T_CHAR: return ("Char");
                case ValueTypes.T_WORD: return ("Word");
                case ValueTypes.T_SHORT: return ("Short");
                case ValueTypes.T_DWORD: return ("DWord");
                case ValueTypes.T_LONG: return ("Long");
                case ValueTypes.T_FLOAT: return ("Float");
                case ValueTypes.T_DOUBLE: return ("Double");
                case ValueTypes.T_DATE: return ("Date");
                case ValueTypes.T_STRING: return ("String");
                case ValueTypes.T_BOOL | ValueTypes.T_ARRAY: return ("Boolean Array");
                case ValueTypes.T_BYTE | ValueTypes.T_ARRAY: return ("Byte Array");
                case ValueTypes.T_CHAR | ValueTypes.T_ARRAY: return ("Char Array");
                case ValueTypes.T_WORD | ValueTypes.T_ARRAY: return ("Word Array");
                case ValueTypes.T_SHORT | ValueTypes.T_ARRAY: return ("Short Array");
                case ValueTypes.T_DWORD | ValueTypes.T_ARRAY: return ("DWord Array");
                case ValueTypes.T_LONG | ValueTypes.T_ARRAY: return ("Long Array");
                case ValueTypes.T_FLOAT | ValueTypes.T_ARRAY: return ("Float Array");
                case ValueTypes.T_DOUBLE | ValueTypes.T_ARRAY: return ("Double Array");
                case ValueTypes.T_STRING | ValueTypes.T_ARRAY: return ("String Array");
                case ValueTypes.T_DATE | ValueTypes.T_ARRAY: return ("Date Array");

                default:
                    return ("Default");
            }
        } 

        public int GetStringSize()
        {
            return (tagStringSize);
        }

        
        public int GetArrayRows()
        {
            return (tagArrayRows);
        }

        
        public int GetArrayCols()
        {
            return (tagArrayCols);
        }

        
        public int GetExtendedSize()
        {
            int elementSize = 0;

            if (tagDataType == ValueTypes.T_STRING)
                return (tagStringSize);

            if (tagDataType == (ValueTypes.T_STRING | ValueTypes.T_ARRAY))
                elementSize = tagStringSize;
            else
                elementSize = Value.SizeOf(tagDataType);

            if (Tag.IsArrayType(tagDataType) && tagArrayRows.Equals(null) ? false : true && tagArrayCols.Equals(null) ? false : true)
                return (tagArrayRows * tagArrayCols * elementSize);

            return (0);
        }


        public VALTYPE GetDataType()
        {
            return (tagDataType);
        }

        
        public AccessType GetReadWrite()
        {
            return (tagReadWrite);
        }


        
        public void GetReadWriteAsText(ref string strReadWrite)
        {
            if (GetReadWrite() == AccessType.READONLY)
                strReadWrite = "Read Only";
            else
                strReadWrite = "Read/Write";
        }

        
        public bool IsReadable()
        {
            AccessType readWrite = GetReadWrite();
            return (readWrite == AccessType.READONLY || readWrite == AccessType.READWRITE);
        }


        public bool IsWriteable()
        {
            AccessType readWrite = GetReadWrite();
            return (readWrite == AccessType.WRITEONLY || readWrite == AccessType.READWRITE);
        }


        public int GetScanRateMilliseconds()
        {
            return (tagScanRateMS);
        }


        public string GetDescription()
        {
            return (tagDescription);
        }


        public string GetGroupName()
        {
            return (tagGroupName);
        }


        public DWORD GetRelativeOffset()
        {
            return (tagRelativeOffset);
        }


        public void SetSharedMemoryOffset(DWORD sharedMemoryOffset)
        {
            tagSharedMemoryOffset = sharedMemoryOffset;
        }


        public DWORD GetSharedMemoryOffset()
        {
            return (tagSharedMemoryOffset);
        }


        public VALTYPE GetReadValueType()
        {
            return (tagReadData.value.GetValueType());
        }


        public VALTYPE GetWriteValueType()
        {
            return (tagWriteData.value.GetValueType());
        }


        public WORD GetReadValueExtSize()
        {
            return (tagReadData.value.valueExtSize);
        }


        public WORD GetWriteValueExtSize()
        {
            return (tagWriteData.value.valueExtSize);
        }


        public WORD GetReadValueArrayStringSize()
        {
            if (tagReadData.value.valueDynamicArray.Length == 0)
            {
                // String array should have been created
                Debug.Assert(false);
                return (0);
            }
            WORD wStrArraySize = tagReadData.value.valueStringSize;
            return (wStrArraySize);
        }


        public WORD GetWriteValueArrayStringSize()
        {
            if (tagWriteData.value.valueDynamicArray.Length == 0)
            {
                // String array should have been created
                Debug.Assert(false);
                return (0);
            }
            WORD wStrArraySize = tagWriteData.value.valueStringSize;
            return (wStrArraySize);
        }

    }

}
