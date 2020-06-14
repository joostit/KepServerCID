// **************************************************************************
// File:  register.cs
// Created:  11/30/2009 Copyright (c) 2009
//
// Description: Defines Shared Memory interface functions and assisting helper 
// functions.
//
// THE FOLLOWING HELPER FUNCTIONS AND THEIR IMPLEMENTATION ARE NOT A REQUIREMENT 
// OF THE CIDA.  SEE CIDA USER DOCUMENTATION FOR COMPLETE SET OF CIDA REQUIREMENTS.
//
// **************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using WORD = System.UInt16;
using DWORD = System.UInt32;
using VALTYPE = System.UInt16;

namespace CidaRefImplCsharp
    {
    public class Register
        {
        public const int READ_OFFSET = 0;
        public const int WRITE_OFFSET = 4;
        public const int DATA_MIN_OFFSET = 12;
        public const int DATA_SIZE_MIN = 30;
        public const int DATA_STATUS_OFFSET = 0;
        public const int DATA_ERRORCODE_OFFSET = 2;
        public const int DATA_QUALITY_OFFSET = 6;
        public const int DATA_TIMESTAMP_OFFSET = 8;
        public const int DATA_VALUE_OFFSET = 16;
        public const int VALUE_TYPE_OFFSET = 0;
        public const int VALUE_8BYTEVAL_OFFSET = 4;
        public const int VALUE_EXTSIZE_OFFSET = 12;
        public const int VALUE_EXTARRAY_OFFSET = 14;

        public const int STRINGARRAY_START = 0;
        public const int STRINGARRAY_STRINGSIZE_OFFSET = STRINGARRAY_START;
        public const int STRINGARRAY_ABDATA_OFFSET = STRINGARRAY_START + 2;


        // The Register class is used for accessing shared memory.

        // *************************************************************************************
        public static void SetWriteValueType(UnmanagedMemoryStream s, long memOffset, WORD writeValueType)
            {
            if (GetRegisterWriteOffset(s, memOffset) != 0)
                {
                SetDataValueType(s, memOffset + GetRegisterWriteOffset(s, memOffset), writeValueType);
                }
            }

        // *************************************************************************************
        public static void SetReadValueType(UnmanagedMemoryStream s, long memOffset, WORD readValueType)
            {
            if (GetRegisterReadOffset(s, memOffset) != 0)
                {
                SetDataValueType(s, memOffset + GetRegisterReadOffset(s, memOffset), readValueType);
                }
            }

        // *************************************************************************************
        public static void SetReadOffset(UnmanagedMemoryStream s, long memOffset, DWORD readOffset)
            {
            SetRegisterReadOffset(s, memOffset, readOffset);
            }

        // *************************************************************************************
        public static void SetWriteOffset(UnmanagedMemoryStream s, long memOffset, DWORD writeOffset)
            {
            SetRegisterWriteOffset(s, memOffset, writeOffset);
            }

        // *************************************************************************************
        public static DWORD GetRegisterReadOffset(UnmanagedMemoryStream s, long memOffset)
            {
            DWORD val;
            BinaryReader r = new BinaryReader(s);
            r.BaseStream.Seek(memOffset + READ_OFFSET, SeekOrigin.Begin); 
            return (val = r.ReadUInt32());
            }

        // *************************************************************************************
        public static DWORD GetRegisterWriteOffset(UnmanagedMemoryStream s, long memOffset)
            {
            DWORD val;
            BinaryReader r = new BinaryReader(s);
            r.BaseStream.Seek(memOffset + WRITE_OFFSET, SeekOrigin.Begin); 
            return (val = r.ReadUInt32());
            }

        // *************************************************************************************
        public static void SetRegisterReadOffset(UnmanagedMemoryStream s, long memOffset, DWORD readOffset)
            {
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset + READ_OFFSET, SeekOrigin.Begin); 
            w.Write(readOffset);
            w.Flush();
            }

        // *************************************************************************************
        public static void SetRegisterWriteOffset(UnmanagedMemoryStream s, long memOffset, DWORD writeOffset)
            {
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset + WRITE_OFFSET, SeekOrigin.Begin); 
            w.Write(writeOffset);
            w.Flush();
            }

        // *************************************************************************************
        public static VALTYPE GetDataValueType(UnmanagedMemoryStream s, long memOffset)
            {
            VALTYPE val;
            BinaryReader r = new BinaryReader(s);
            r.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_TYPE_OFFSET, SeekOrigin.Begin); 
            return (val = r.ReadUInt16());
            }


        // *************************************************************************************
        public static void SetDataValueType(UnmanagedMemoryStream s, long memOffset, WORD type)
            {
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_TYPE_OFFSET, SeekOrigin.Begin); 
            w.Write(type);
            w.Flush();
            }

        public static void SetReadValueExtSize(UnmanagedMemoryStream s, long memOffset, WORD extSize)
            {
            if (GetRegisterReadOffset(s, memOffset) != 0)
                {
                SetDataValueExtSize(s, memOffset + GetRegisterReadOffset(s, memOffset), extSize);
                }
            }

        // *************************************************************************************
        public static void SetWriteValueExtSize(UnmanagedMemoryStream s, long memOffset, WORD extSize)
            {
            if (GetRegisterWriteOffset(s, memOffset) != 0)
                {
                SetDataValueExtSize(s, memOffset + GetRegisterWriteOffset(s, memOffset), extSize);
                }
            }

        // *************************************************************************************
        public static void SetDataValueExtSize(UnmanagedMemoryStream s, long memOffset, WORD extSize)
            {
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_EXTSIZE_OFFSET, SeekOrigin.Begin); 
            w.Write(extSize);
            w.Flush();
            }

        // *************************************************************************************
        public static void GetReadResponsePending(UnmanagedMemoryStream s, long memOffset, ref bool bReadResponsePending)
            {
            if (GetRegisterReadOffset(s, memOffset) != 0)
                {
                bReadResponsePending = GetDataResponsePending(s, memOffset + GetRegisterReadOffset(s, memOffset));
                }
            }

        // *************************************************************************************
        public static void GetWriteResponsePending(UnmanagedMemoryStream s, long memOffset, ref bool bWriteResponsePending)
            {
            if (GetRegisterWriteOffset(s, memOffset) != 0)
                {
                bWriteResponsePending = GetDataResponsePending(s, memOffset + GetRegisterWriteOffset(s, memOffset));
                }
            }

        // *************************************************************************************
        public static bool GetDataResponsePending(UnmanagedMemoryStream s, long memOffset)
            {
            BinaryReader r = new BinaryReader(s);
            r.BaseStream.Seek(memOffset + DATA_STATUS_OFFSET, SeekOrigin.Begin); 
            return ((r.ReadUInt16() & (int)TagData.STS_RESPONSEPENDING) == (int)TagData.STS_RESPONSEPENDING ? true : false);
            }

        // *************************************************************************************
        public static void SetReadResponse(UnmanagedMemoryStream s, long memOffset, Value value, bool error, DWORD errorCode, WORD quality, FILETIME timeStamp)
            {
            if (GetRegisterReadOffset(s, memOffset) != 0)
                {
                SetReadResponseData(s, memOffset + GetRegisterReadOffset(s, memOffset), value, error, errorCode, quality, timeStamp);
                }
            SetDataResponsePending(s, memOffset + GetRegisterReadOffset(s, memOffset), true);
            }

        // *************************************************************************************
        public static int SetReadResponseData(UnmanagedMemoryStream s, long memOffset, Value value, bool error, DWORD errorCode, WORD quality, FILETIME timeStamp)
            {
            VALTYPE tType = GetDataValueType(s, memOffset);
            if (!Value.IsValidValueType(tType))
                {
                return (TagData.SMRC_BAD_VALTYPE);
                }
            if (error)
                {
                SetDataError(s, memOffset, errorCode, true);
                }
            else
                {
                SetDataError(s, memOffset, errorCode, false);
                }

            SetDataQuality(s, memOffset, quality);
            SetDataTimestamp(s, memOffset, timeStamp);
            SetDataValue(s, memOffset, value);

            return (TagData.SMRC_NO_ERROR);
            }


        // *************************************************************************************
        public static void SetWriteResponse(UnmanagedMemoryStream s, long memOffset, bool error, DWORD errorCode, WORD quality, FILETIME timeStamp)
            {
            if (GetRegisterWriteOffset(s, memOffset) != 0)
                {
                SetWriteResponseData(s, memOffset + GetRegisterWriteOffset(s, memOffset), error, errorCode, quality, timeStamp);
                }
            SetDataResponsePending(s, memOffset + GetRegisterWriteOffset(s, memOffset), true);
            }

        // *************************************************************************************
        public static int SetWriteResponseData(UnmanagedMemoryStream s, long memOffset, bool error, DWORD errorCode, WORD quality, FILETIME timeStamp)
            {
            VALTYPE tType = GetDataValueType(s, memOffset);
            if (!Value.IsValidValueType(tType))
                {
                return (TagData.SMRC_BAD_VALTYPE);
                }
            if (error)
                {
                SetDataError(s, memOffset, errorCode, true);
                }
            else
                {
                SetDataError(s, memOffset, errorCode, false);
                }

            SetDataQuality(s, memOffset, quality);
            SetDataTimestamp(s, memOffset, timeStamp);
            return (TagData.SMRC_NO_ERROR);
            }


        // *************************************************************************************
        public static void SetDataError(UnmanagedMemoryStream s, long memOffset, DWORD errorCode, bool bFlag)
            {
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset + DATA_ERRORCODE_OFFSET, SeekOrigin.Begin); 
            w.Write(errorCode);
            // flag/clear the error in status
            w.BaseStream.Seek(memOffset, SeekOrigin.Begin); 
            WORD status = GetDataStatus(s, memOffset);
            if (bFlag)
                {
                status |= TagData.STS_ERROR;
                }
            else
                {
                status = (WORD) (status & ~TagData.STS_ERROR);
                }
            //reposition the stream to write
            w.BaseStream.Seek(memOffset, SeekOrigin.Begin); 
            w.Write(status);
            w.Flush();
            }

        // *************************************************************************************
        public static void SetDataQuality(UnmanagedMemoryStream s, long memOffset, WORD quality)
            {
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset + DATA_QUALITY_OFFSET, SeekOrigin.Begin); 
            w.Write(quality);
            w.Flush();
            }

        // *************************************************************************************
        public static WORD GetDataStatus(UnmanagedMemoryStream s, long memOffset)
            {
            WORD val;
            BinaryReader r = new BinaryReader(s);
            r.BaseStream.Seek(memOffset + DATA_STATUS_OFFSET, SeekOrigin.Begin); 
            return (val = r.ReadUInt16());
            }

        // *************************************************************************************
        public static WORD GetDataQuality(UnmanagedMemoryStream s, long memOffset)
            {
            WORD val;
            BinaryReader r = new BinaryReader(s);
            r.BaseStream.Seek(memOffset + DATA_QUALITY_OFFSET, SeekOrigin.Begin); 
            return (val = r.ReadUInt16());
            }

        // *************************************************************************************
        public static WORD GetReadRequest(UnmanagedMemoryStream s, long memOffset)
            {
            if (GetRegisterReadOffset(s, memOffset) == 0)
                {
                return TagData.SMRC_NO_DATA;
                }

            SetDataRequestPending(s, memOffset + GetRegisterReadOffset(s, memOffset), false);
            return (TagData.SMRC_NO_ERROR);
            }

        // *************************************************************************************
        public static void SetDataRequestPending(UnmanagedMemoryStream s, long memOffset, bool bFlag)
            {
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset, SeekOrigin.Begin); 
            WORD status = GetDataStatus(s, memOffset);
            if (bFlag)
                {
                status |= TagData.STS_REQUESTPENDING ;
                }
            else
                {
                status = (WORD) (status & ~TagData.STS_REQUESTPENDING );
                }
            //reposition the stream to write
            w.BaseStream.Seek(memOffset, SeekOrigin.Begin); 
            w.Write(status);
            w.Flush();
            }

        // *************************************************************************************
        static public int GetWriteRequestPending(UnmanagedMemoryStream s, long memOffset, ref bool bFlag)
            {
            if (GetRegisterWriteOffset(s, memOffset) == 0)
                {
                return TagData.SMRC_NO_DATA;
                }

            bFlag = GetDataRequestPending(s, memOffset + GetRegisterWriteOffset(s, memOffset));
            return (TagData.SMRC_NO_ERROR);
            }

        // *************************************************************************************
        public static bool GetDataRequestPending(UnmanagedMemoryStream s, long memOffset)
            {
            BinaryReader r = new BinaryReader(s);
            r.BaseStream.Seek(memOffset, SeekOrigin.Begin); 
            WORD status = GetDataStatus(s, memOffset);

            return (((status & TagData.STS_REQUESTPENDING) != TagData.FALSE) ? true : false);
            }

        // *************************************************************************************
        static public int GetWriteRequest(UnmanagedMemoryStream s, long memOffset, ref  Value value, ref WORD quality, ref FILETIME timeStamp)
            {
            if (GetRegisterWriteOffset(s, memOffset) == 0)
                {
                return TagData.SMRC_NO_DATA;
                }

            int nRC = GetWriteRequestData(s, memOffset + GetRegisterWriteOffset(s, memOffset), ref value, ref quality, ref timeStamp);
            SetDataRequestPending(s, memOffset + GetRegisterWriteOffset(s, memOffset), false);
            return (nRC);
            }

        // *************************************************************************************
        static public int GetWriteRequestData(UnmanagedMemoryStream s, long memOffset, ref Value value, ref WORD quality, ref FILETIME timeStamp)
            {

            VALTYPE tType = GetDataValueType(s, memOffset);
            if (!Value.IsValidValueType(tType))
                {
                return (TagData.SMRC_BAD_VALTYPE);
                }

            quality = Register.GetDataQuality(s, memOffset);
            timeStamp = Register.GetDataTimestamp(s, memOffset + DATA_VALUE_OFFSET);
            GetDataValue(s, memOffset, ref value);
            return (TagData.SMRC_NO_ERROR);
            }

        // *************************************************************************************
        static public int GetReadRequestPending(UnmanagedMemoryStream s, long memOffset, ref bool bFlag)
            {
            if (GetRegisterReadOffset(s, memOffset) == 0)
                {
                return TagData.SMRC_NO_DATA;
                }

            bFlag = GetDataRequestPending(s, memOffset + GetRegisterReadOffset(s, memOffset));
            return (TagData.SMRC_NO_ERROR);
            }


        // *************************************************************************************
        static public int SetDataValue(UnmanagedMemoryStream s, long memOffset, Value value)
            {
            BinaryWriter w = new BinaryWriter(s);

            VALTYPE vType = value.GetValueType();
            if (((vType & Value.T_ARRAY) == Value.T_ARRAY) && ((vType & ~Value.T_ARRAY)!= Value.T_STRING))//array, not string array
                {
                w.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_EXTARRAY_OFFSET, SeekOrigin.Begin);
                }
            else if (vType == (Value.T_ARRAY | Value.T_STRING)) //string array
                {
                w.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_EXTARRAY_OFFSET + STRINGARRAY_ABDATA_OFFSET, SeekOrigin.Begin);
                }
            else if (vType == Value.T_STRING) //string
                {
                w.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_EXTARRAY_OFFSET, SeekOrigin.Begin);
                }
            else
                {
                w.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_8BYTEVAL_OFFSET, SeekOrigin.Begin); 
                }
            byte[] aToWrite = value.GetArrayFromValue();
            w.Write(aToWrite);
            w.Flush();
            return (TagData.SMRC_NO_ERROR);
            }

        // *************************************************************************************
        static public void GetDataValue(UnmanagedMemoryStream s, long memOffset, ref Value value)
            {
            BinaryReader r = new BinaryReader(s);

            VALTYPE vType = value.GetValueType();
            if (((vType & Value.T_ARRAY) == Value.T_ARRAY) && ((vType & ~Value.T_ARRAY) != Value.T_STRING))//array, not string array
                {
                r.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_EXTARRAY_OFFSET, SeekOrigin.Begin);
                }
            else if (vType == (Value.T_ARRAY | Value.T_STRING)) //string array
                {
                r.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_EXTARRAY_OFFSET + STRINGARRAY_ABDATA_OFFSET, SeekOrigin.Begin);
                }
            else if (vType == Value.T_STRING) //string
                {
                r.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_EXTARRAY_OFFSET, SeekOrigin.Begin);
                }
            else
                {
                r.BaseStream.Seek(memOffset + DATA_VALUE_OFFSET + VALUE_8BYTEVAL_OFFSET, SeekOrigin.Begin);
                }

            if (value.valueExtSize > 0)
                {
                r.Read(value.valueExtByteArray, 0, value.valueExtByteArray.Length);
                }
            else
                {
                r.Read(value.value8ByteArray, 0, value.value8ByteArray.Length);
                }
            value.SetValueFromArray();
            }

        // *************************************************************************************
        public static void SetDataResponsePending(UnmanagedMemoryStream s, long memOffset, bool bFlag)
            {
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset, SeekOrigin.Begin); 
            WORD status = GetDataStatus(s, memOffset);
            if (bFlag)
                {
                status |= TagData.STS_RESPONSEPENDING;
                }
            else
                {
                status = (WORD) (status & ~TagData.STS_RESPONSEPENDING);
                }
            //reposition the stream to write
            w.BaseStream.Seek(memOffset, SeekOrigin.Begin); 
            w.Write(status);
            w.Flush();
            }

        // *************************************************************************************
        public static void SetDataTimestamp(UnmanagedMemoryStream s, long memOffset, FILETIME timeStamp)
            {
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset + DATA_TIMESTAMP_OFFSET, SeekOrigin.Begin);
            w.Write(timeStamp.dwLowDateTime);
            w.Write(timeStamp.dwHighDateTime);
            w.Flush();
            }

        // *************************************************************************************
        public static FILETIME GetDataTimestamp(UnmanagedMemoryStream s, long memOffset)
            {
            FILETIME ft;
            BinaryReader r = new BinaryReader(s);
            r.BaseStream.Seek(memOffset + DATA_TIMESTAMP_OFFSET, SeekOrigin.Begin);
            ft.dwLowDateTime = (DWORD)r.ReadDouble();
            ft.dwHighDateTime = (DWORD)r.ReadDouble();
            return (ft);
            }

        // *************************************************************************************
        static public int SetReadValueArrayStringSize(UnmanagedMemoryStream s, long memOffset, WORD wSize)
            {
            if (GetRegisterReadOffset(s, memOffset) == 0)
                {
                return TagData.SMRC_NO_DATA;
                }
            SetDataValueArrayStringSize(s, memOffset + GetRegisterReadOffset(s, memOffset), wSize);
            return (TagData.SMRC_NO_ERROR);
            }

        // *************************************************************************************
        static public int SetWriteValueArrayStringSize(UnmanagedMemoryStream s, long memOffset, WORD wSize)
            {
            if (GetRegisterWriteOffset(s, memOffset) == 0)
                {
                return TagData.SMRC_NO_DATA;
                }
            SetDataValueArrayStringSize(s, memOffset + GetRegisterWriteOffset(s, memOffset), wSize);
            return (TagData.SMRC_NO_ERROR);
            }

        // *************************************************************************************
        public static void SetDataValueArrayStringSize(UnmanagedMemoryStream s, long memOffset, WORD wSize)
            {
            SetValueArrayStringSize(s, memOffset + DATA_VALUE_OFFSET, wSize);
            }

        // *************************************************************************************
        public static void SetValueArrayStringSize(UnmanagedMemoryStream s, long memOffset, WORD wSize)
            {
            //The STRINGARRAY struct starts at the abExtValue offset of the VALUE struct
            BinaryWriter w = new BinaryWriter(s);
            w.BaseStream.Seek(memOffset + VALUE_EXTARRAY_OFFSET + STRINGARRAY_STRINGSIZE_OFFSET, SeekOrigin.Begin); 
            w.Write(wSize);
            w.Flush();
            }

        // *************************************************************************************
        public static WORD GetValueStringArraySize(UnmanagedMemoryStream s, DWORD sharedMemoryOffset)
            {
            long memOffset = (long)sharedMemoryOffset;
            WORD wVal;
            if (GetRegisterReadOffset(s, memOffset) == 0)
                {
                return TagData.SMRC_NO_DATA;
                }
            BinaryReader r = new BinaryReader(s);
            r.BaseStream.Seek(memOffset + GetRegisterReadOffset(s, memOffset) + DATA_VALUE_OFFSET
                + VALUE_EXTARRAY_OFFSET + STRINGARRAY_STRINGSIZE_OFFSET, SeekOrigin.Begin);
            wVal = r.ReadUInt16();
            return (wVal);
            }

        } // public class Register

    } // namespace CidaRefImplCsharp
