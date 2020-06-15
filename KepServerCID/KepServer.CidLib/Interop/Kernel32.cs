using KepServer.CidLib.Interop.WinBASE;
using KepServer.CidLib.Interop.WinNT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace KepServer.CidLib.Interop
{
    internal static class Kernel32
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetTickCount();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile,
                                                int lpAttributes,
                                                FileProtection flProtect,
                                                uint dwMaximumSizeHigh,
                                                uint dwMaximumSizeLow,
                                                string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenFileMapping(FileRights dwDesiredAccess,
                                              bool bInheritHandle,
                                              string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject,
                                            FileRights dwDesiredAccess,
                                            uint dwFileOffsetHigh,
                                            uint dwFileOffsetLow,
                                            uint dwNumberOfBytesToMap);
        [DllImport("Kernel32.dll")]
        public static extern bool UnmapViewOfFile(IntPtr map);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);

    }
}
