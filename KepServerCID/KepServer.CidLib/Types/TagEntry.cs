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

namespace KepServer.CidLib.Types
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct TagEntry
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
        public TagEntry(string _strName, WORD _stringSize, WORD _arrayRows,
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
        public TagEntry(TagEntry te)
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
}
