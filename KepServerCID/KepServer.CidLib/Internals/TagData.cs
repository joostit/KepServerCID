// **************************************************************************
// File:  tag.cs
// Created:  11/30/2009 Copyright (c) 2009
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

namespace KepServer.CidLib.Internals
{


    public class TagData
    {

        internal const int OPC_NO_QUALITY_NO_VALUE = 0xFF;

        internal const int OPC_QUALITY_BAD_NON_SPECIFIC = 0x0;
        internal const int OPC_QUALITY_BAD_CONFIG_ERR0R = 0x04;
        internal const int OPC_QUALITY_BAD_NOT_CONNECTED = 0x08;
        internal const int OPC_QUALITY_BAD_DEVICE_FAILURE = 0x0C;
        internal const int OPC_QUALITY_BAD_SENSOR_FAILURE = 0x10;
        internal const int OPC_QUALITY_BAD_LAST_KNOWN_VALUE = 0x14;
        internal const int OPC_QUALITY_BAD_COMM_FAILURE = 0x18;
        internal const int OPC_QUALITY_BAD_OUT_OF_SERVICE = 0x1C;
        internal const int OPC_QUALITY_WAITING_FOR_INITIAL_DATA = 0x20;

        internal const int OPC_QUALITY_UNCERTAIN_NON_SPECIFIC = 0x40;
        internal const int OPC_QUALITY_UNCERTAIN_LAST_USABLE_VALUE = 0x44;
        internal const int OPC_QUALITY_UNCERTAIN_SENSOR_NOT_ACCURATE = 0x50;
        internal const int OPC_QUALITY_UNCERTAIN_EU_UNITS_EXCEEDED = 0x54;
        internal const int OPC_QUALITY_UNCERTAIN_SUB_NORMAL = 0x58;

        internal const int OPC_QUALITY_GOOD_NON_SPECIFIC = 0xC0;
        internal const int OPC_QUALITY_GOOD_LOCAL_OVERRIDE = 0xD8;

        internal const int OPC_QUALITY_LIMITFIELD_NOT = 0x0;
        internal const int OPC_QUALITY_LIMITFIELD_LOW = 0x1;
        internal const int OPC_QUALITY_LIMITFIELD_HIGH = 0x2;
        internal const int OPC_QUALITY_LIMITFIELD_CONSTANT = 0x3;

        internal const int TRUE = 1;
        internal const int FALSE = 0;

        // STATUS masks (&&  or || with UInt16 status)
        internal const UInt16 STS_REQUESTPENDING = 1;
        internal const UInt16 STS_RESPONSEPENDING = 2;
        internal const UInt16 STS_ERROR = 4;

        // Shared Memory Return Codes
        internal const int SMRC_NO_ERROR = 0;
        internal const int SMRC_INVALID_PTR = 1;
        internal const int SMRC_NO_DATA = 2;
        internal const int SMRC_BAD_VALTYPE = 3;

        public DWORD errorCode = 0;
        public WORD quality = TagData.OPC_QUALITY_BAD_OUT_OF_SERVICE;
        public FileTime timeStamp = new FileTime();
        public Value value;


        internal TagData(MemInterface memInterface, VALTYPE vType, WORD stringSize, int rows, int cols)
        {
            value = new Value(vType, stringSize, rows, cols);
            memInterface.GetFtNow(ref timeStamp);
        }


        public VALTYPE GetValueType()
        {
            return (value.GetValueType());
        }

    }

} 
