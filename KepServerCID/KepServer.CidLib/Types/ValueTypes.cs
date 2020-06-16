using System;
using System.Collections.Generic;
using System.Text;
using VALTYPE = System.UInt16;

namespace KepServer.CidLib.Types
{
    public static class ValueTypes
    {
        public const VALTYPE T_UNDEFINED = 0;
        public const VALTYPE T_BOOL = 1;
        public const VALTYPE T_BYTE = 2;
        public const VALTYPE T_CHAR = 3;
        public const VALTYPE T_WORD = 4;
        public const VALTYPE T_SHORT = 5;
        public const VALTYPE T_DWORD = 6;
        public const VALTYPE T_LONG = 7;
        public const VALTYPE T_FLOAT = 8;
        public const VALTYPE T_DOUBLE = 9;
        public const VALTYPE T_DATE = 10;
        public const VALTYPE T_STRING = 11;
        public const VALTYPE T_ARRAY = 0x1000;
    }
}
