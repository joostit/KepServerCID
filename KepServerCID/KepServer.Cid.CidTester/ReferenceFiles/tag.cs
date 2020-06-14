// **************************************************************************
// File:  tag.cs
// Created:  11/30/2009 Copyright (c) 2009
//
// Description:  Defines the Tag class.  The Tag class includes the
// name of the tag, its datatype, and its offset within Shared Memory to name
// a few.  It also maintains a read data class and write data class.
// These data classes are separate from the Shared Memory DATA byte array and
// are meant to store locally, the value from a "read response" and the value
// for a "write request".  In a commerical application these values would be
// used in communicating with an actual device. The Device is responsible
// for exporting its definition to the Configuration file when requested.
//
// **************************************************************************
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

namespace CidaRefImplCsharp
    {

    // Using a local definition of FILETIME with unsigned members
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct FILETIME
        {
        public UInt32 dwLowDateTime;
        public UInt32 dwHighDateTime;
        }

    public enum AccessType
        {
        READONLY = 0,
        READWRITE,
        WRITEONLY
        }

    // *************************************************************************************
    public class TagData
        {

        public const int OPC_NO_QUALITY_NO_VALUE = 0xFF;

        public const int OPC_QUALITY_BAD_NON_SPECIFIC = 0x0;
        public const int OPC_QUALITY_BAD_CONFIG_ERR0R = 0x04;
        public const int OPC_QUALITY_BAD_NOT_CONNECTED = 0x08;
        public const int OPC_QUALITY_BAD_DEVICE_FAILURE = 0x0C;
        public const int OPC_QUALITY_BAD_SENSOR_FAILURE = 0x10;
        public const int OPC_QUALITY_BAD_LAST_KNOWN_VALUE = 0x14;
        public const int OPC_QUALITY_BAD_COMM_FAILURE = 0x18;
        public const int OPC_QUALITY_BAD_OUT_OF_SERVICE = 0x1C;
        public const int OPC_QUALITY_WAITING_FOR_INITIAL_DATA = 0x20;

        public const int OPC_QUALITY_UNCERTAIN_NON_SPECIFIC = 0x40;
        public const int OPC_QUALITY_UNCERTAIN_LAST_USABLE_VALUE = 0x44;
        public const int OPC_QUALITY_UNCERTAIN_SENSOR_NOT_ACCURATE = 0x50;
        public const int OPC_QUALITY_UNCERTAIN_EU_UNITS_EXCEEDED = 0x54;
        public const int OPC_QUALITY_UNCERTAIN_SUB_NORMAL = 0x58;

        public const int OPC_QUALITY_GOOD_NON_SPECIFIC = 0xC0;
        public const int OPC_QUALITY_GOOD_LOCAL_OVERRIDE = 0xD8;

        public const int OPC_QUALITY_LIMITFIELD_NOT = 0x0;
        public const int OPC_QUALITY_LIMITFIELD_LOW = 0x1;
        public const int OPC_QUALITY_LIMITFIELD_HIGH = 0x2;
        public const int OPC_QUALITY_LIMITFIELD_CONSTANT = 0x3;

        public const int TRUE = 1;
        public const int FALSE = 0;

        // STATUS masks (&&  or || with UInt16 status)
        public const UInt16 STS_REQUESTPENDING = 1;
        public const UInt16 STS_RESPONSEPENDING = 2;
        public const UInt16 STS_ERROR = 4;

        // Shared Memory Return Codes
        public const int SMRC_NO_ERROR = 0;
        public const int SMRC_INVALID_PTR = 1;
        public const int SMRC_NO_DATA = 2;
        public const int SMRC_BAD_VALTYPE = 3;

        public WORD status = 0;
        public DWORD errorCode = 0;
        public WORD quality = TagData.OPC_QUALITY_BAD_OUT_OF_SERVICE;
        public FILETIME timeStamp = new FILETIME();
        public Value value;


        // *************************************************************************************
        public TagData(DWORD memOffset, VALTYPE vType, WORD stringSize, int rows, int cols)
            {
            value = new Value(vType, stringSize, rows, cols);
            MemInterface.GetFtNow(ref timeStamp);
            }

        // *************************************************************************************
        public TagData(VALTYPE vType, WORD stringSize, int rows, int cols)
            {
            value = new Value(vType, stringSize, rows, cols);
            MemInterface.GetFtNow(ref timeStamp);
            }

        // *************************************************************************************
        public VALTYPE GetValueType()
            {
            return (value.GetValueType());
            }

        } // class TagData


        // *************************************************************************************
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
        public DWORD tagRelativeOffset;			// Register's offset relative to device's shared memory offset
        public DWORD tagSharedMemoryOffset;		// Register's absolute offset in shared memory

        public bool tagWriteRequestPending;
        public bool tagReadRequestPending;
        public bool tagWriteResponsePending;
        public bool tagReadResponsePending;

        public Device tagDevice; //reference to the device that owns this tag

        public TagData tagReadData;
        public TagData tagWriteData;

        // *************************************************************************************
        // Name
        public string GetName()
            {
            return (tagName);
            }

        public DWORD readOffset;
        public DWORD writeOffset;
        public DWORD regReserved12;

        // *************************************************************************************
        public Tag(DWORD _readOffset, DWORD _writeOffset, VALTYPE vType,
            WORD stringSize, int rows, int cols)
            {
            readOffset = _readOffset;
            writeOffset = _writeOffset;
            tagReadData = new TagData(_readOffset, vType, stringSize, rows, cols);
            if (writeOffset != 0)
                {
                tagWriteData = new TagData(_writeOffset, vType, stringSize, rows, cols);
                }
            }

        // *************************************************************************************
        public Tag(ref Device device, TAGENTRY tTagEntry, DWORD dwRelativeOffset, DWORD dwParentOffset)
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
                tagReadData = new TagData(tTagEntry.dataType,
					tTagEntry.stringSize, tTagEntry.arrayRows, tTagEntry.arrayCols);
                }

            if ((tTagEntry.Access == AccessType.READWRITE ) ||
                (tTagEntry.Access == AccessType.WRITEONLY ))
                {
                tagWriteData = new TagData(tTagEntry.dataType,
					tTagEntry.stringSize, tTagEntry.arrayRows, tTagEntry.arrayCols);
                }

            } // Tag constructor for AddTag


        // *************************************************************************************
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TAGENTRY
            {
            public string strName;
            public WORD stringSize;
            public WORD arrayRows;
            public WORD arrayCols;
            public VALTYPE dataType;
            public AccessType Access;
            public string description;
            public string groupName;

            // *************************************************************************************
            public TAGENTRY(string _strName, WORD _stringSize, WORD _arrayRows,
                WORD _arrayCols, VALTYPE _dataType, AccessType _Access,
                string _description, string _groupName)
                {
                strName = _strName;
				stringSize = _stringSize;
				arrayRows = _arrayRows;
				arrayCols = _arrayCols;
                dataType = _dataType;
                Access = _Access;
				description = _description;
				groupName = _groupName;
                }

            // *************************************************************************************
            public TAGENTRY(Tag.TAGENTRY te)
                {
                strName = te.strName;
				stringSize = te.stringSize;
				arrayRows = te.arrayRows;
				arrayCols = te.arrayCols;
                dataType = te.dataType;
                Access = te.Access;
				description = te.description;
				groupName = te.groupName;
                }

            } // struct TAGENTRY

        // **************************************************************************
        // GetCIDAddress
        // Purpose: Format an address to meet the expectation of the CID driver (e.g. D0)
        // This is not to be confused with a PLC address.
        // **************************************************************************
        public void GetCIDAddress(ref string strAddress)
            {
            // Format register number
            string strBuffer = string.Format("D{0,0:D}", tagRelativeOffset);
            if (strBuffer.Length > 16)
                strBuffer = strBuffer.Substring(0, 16);
            strAddress = strBuffer;

            string strExtBuffer = "";

            if (tagDataType == Value.T_STRING)
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
                        if ((tagDataType & ~Value.T_ARRAY) == Value.T_STRING)
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
                    Debug.Assert((tagDataType & ~Value.T_ARRAY) != Value.T_STRING);	// // Multi-dimensional string arrays are not allowed
                    strExtBuffer = string.Format("[{0,0:D}] [{1,0:D}]", tagArrayRows, tagArrayCols);
                    if (strExtBuffer.Length > 32)
                        {
                        strExtBuffer = strExtBuffer.Substring(0, 32);
                        }
                    }
                strAddress += strExtBuffer;
                }
            } // GetCIDAddress(ref string strAddress)


        // *************************************************************************************
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
            } // ExportConfiguration(string strConfigFile)

        // *************************************************************************************
        // This is how we make an array type from a basic type.
        public static VALTYPE MakeArrayType(VALTYPE dataType)
            {
            return (VALTYPE)(dataType | Value.T_ARRAY);
            }

        // *************************************************************************************
        public static VALTYPE MakeBasicType(VALTYPE dataType)
            {
            return (VALTYPE)(dataType & ~Value.T_ARRAY);
            }

        // *************************************************************************************
        public static bool IsArrayType(VALTYPE dataType) 
	        {
            return ((dataType & Value.T_ARRAY) == Value.T_ARRAY ? true : false);
            }

        // *************************************************************************************
        public static string TextFromType(VALTYPE valueType)
            {
            switch (valueType)
	            {
	            case Value.T_BOOL: return ("Boolean");
	            case Value.T_BYTE: return ("Byte");
	            case Value.T_CHAR: return ("Char");
	            case Value.T_WORD: return ("Word");
	            case Value.T_SHORT: return ("Short");
	            case Value.T_DWORD: return ("DWord");
	            case Value.T_LONG: return ("Long");
	            case Value.T_FLOAT: return ("Float");
	            case Value.T_DOUBLE: return ("Double");
	            case Value.T_DATE: return ("Date");
	            case Value.T_STRING: return ("String");
	            case Value.T_BOOL|Value.T_ARRAY: return ("Boolean Array");
	            case Value.T_BYTE|Value.T_ARRAY: return ("Byte Array");
	            case Value.T_CHAR|Value.T_ARRAY: return ("Char Array");
	            case Value.T_WORD|Value.T_ARRAY: return ("Word Array");
	            case Value.T_SHORT|Value.T_ARRAY: return ("Short Array");
	            case Value.T_DWORD|Value.T_ARRAY: return ("DWord Array");
	            case Value.T_LONG|Value.T_ARRAY: return ("Long Array");
	            case Value.T_FLOAT|Value.T_ARRAY: return ("Float Array");
	            case Value.T_DOUBLE|Value.T_ARRAY: return ("Double Array");
	            case Value.T_STRING|Value.T_ARRAY: return ("String Array");
                case Value.T_DATE | Value.T_ARRAY: return ("Date Array");

	            default:
		            return ("Default");
	            }
            } // TextFromType(VALTYPE valueType)

        // *************************************************************************************
        public int GetStringSize()
            {
            return (tagStringSize);
            }

        // *************************************************************************************
        public int GetArrayRows()
            {
            return (tagArrayRows);
            }

        // *************************************************************************************
        public int GetArrayCols()
            {
            return (tagArrayCols);
            }

        // *************************************************************************************
        public int GetExtendedSize()
            {
            int elementSize = 0;

            if (tagDataType == Value.T_STRING)
                return (tagStringSize);

            if (tagDataType == (Value.T_STRING|Value.T_ARRAY))
                elementSize = tagStringSize;
            else
                elementSize =  Value.SizeOf(tagDataType);

            if (Tag.IsArrayType(tagDataType) && tagArrayRows.Equals(null) ? false : true && tagArrayCols.Equals(null) ? false : true)
                return (tagArrayRows * tagArrayCols * elementSize);

            return (0);
            }


        // *************************************************************************************
        // Data Type
        public VALTYPE GetDataType()
            {
            return (tagDataType);
            }

        // *************************************************************************************
        // Read/Write
        public AccessType GetReadWrite()
            {
            return (tagReadWrite);
            }


        // *************************************************************************************
        public void GetReadWriteAsText(ref string strReadWrite)
            {
            if (GetReadWrite() == AccessType.READONLY)
                strReadWrite = "Read Only";
            else
                strReadWrite = "Read/Write";
            }

        // *************************************************************************************
        public bool IsReadable()
            {
            AccessType readWrite = GetReadWrite();
            return (readWrite == AccessType.READONLY || readWrite == AccessType.READWRITE);
            }

        // *************************************************************************************
        public bool IsWriteable()
            {
            AccessType readWrite = GetReadWrite();
            return (readWrite == AccessType.WRITEONLY || readWrite == AccessType.READWRITE);
            }

        // *************************************************************************************
        // Scan Rate
        public int GetScanRateMilliseconds()
            {
            return (tagScanRateMS);
            }

        // *************************************************************************************
        // Description
        public string GetDescription()
            {
            return (tagDescription);
            }

        // *************************************************************************************
        // Group Name
        public string GetGroupName()
            {
            return (tagGroupName);
            }

        // *************************************************************************************
        public DWORD GetRelativeOffset()
            {
            return (tagRelativeOffset);
            }

        // *************************************************************************************
        // Shared Memory Offset
        public void SetSharedMemoryOffset(DWORD sharedMemoryOffset)
            {
            tagSharedMemoryOffset = sharedMemoryOffset;
            }

        // *************************************************************************************
        public DWORD GetSharedMemoryOffset()
            {
            return (tagSharedMemoryOffset);
            }

        // ReadData Value Type
        // *************************************************************************************
        public VALTYPE GetReadValueType()
            {
            return (tagReadData.value.GetValueType());
            }

        // *************************************************************************************
        // WriteData Value Type
        public VALTYPE GetWriteValueType()
            {
            return (tagWriteData.value.GetValueType());
            }

        // *************************************************************************************
        // ReadData Extended Size
        public WORD GetReadValueExtSize()
            {
            return (tagReadData.value.valueExtSize);
            }

        // *************************************************************************************
        // WriteData Extended Size
        public WORD GetWriteValueExtSize()
            {
            return (tagWriteData.value.valueExtSize);
            }

        // *************************************************************************************
        // ReadData Array String Size
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

        // *************************************************************************************
        // WriteData Array String Size
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

        } // class Tag

    } // namespace CidaRefImplCsharp
