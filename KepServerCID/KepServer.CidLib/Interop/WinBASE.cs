using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Interop.WinBASE
{
    public enum FileRights : uint          // constants from WinBASE.h
    {
        Read = 4,
        Write = 2,
        ReadWrite = Read + Write
    }
}
