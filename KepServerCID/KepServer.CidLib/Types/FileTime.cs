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

    // Using a local definition of FILETIME with unsigned members
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct FileTime
    {
        public UInt32 dwLowDateTime;
        public UInt32 dwHighDateTime;
    }
}
