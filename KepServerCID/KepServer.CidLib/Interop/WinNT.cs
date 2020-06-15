using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Interop.WinNT
{
    public enum FileProtection : uint      // constants from winnt.h
    {
        ReadOnly = 2,
        ReadWrite = 4
    }
}
