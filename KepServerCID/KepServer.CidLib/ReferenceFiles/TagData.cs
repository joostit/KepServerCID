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
using KepServer.CidLib.Types;

namespace CidaRefImplCsharp
{

    

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
        public FileTime timeStamp = new FileTime();
        public Value value;
        private MemInterface memInterface;


        // *************************************************************************************
        public TagData(MemInterface memInterface, DWORD memOffset, VALTYPE vType, WORD stringSize, int rows, int cols)
        {
            this.memInterface = memInterface;
            value = new Value(vType, stringSize, rows, cols);
            memInterface.GetFtNow(ref timeStamp);
        }

        // *************************************************************************************
        public TagData(MemInterface memInterface, VALTYPE vType, WORD stringSize, int rows, int cols)
        {
            this.memInterface = memInterface;
            value = new Value(vType, stringSize, rows, cols);
            memInterface.GetFtNow(ref timeStamp);
        }

        // *************************************************************************************
        public VALTYPE GetValueType()
        {
            return (value.GetValueType());
        }

    } // class TagData


    // *************************************************************************************

} // namespace CidaRefImplCsharp
