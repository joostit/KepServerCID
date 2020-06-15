// **************************************************************************
// File:  sharedmemoryserver.cs
// Created:  11/30/2009 Copyright (c) 2009
//
// Description: The Shared Memory Server is responsible for creating (if one
// does not exist) or opening a Shared Memory file.
//
// **************************************************************************
#define TRACE_SM_ACCESS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Security.AccessControl;

using WORD = System.UInt16;
using DWORD = System.UInt32;
using KepServer.CidLib.Interop.WinBASE;
using KepServer.CidLib.Interop;
using KepServer.CidLib.Interop.WinNT;

namespace KepServer.CidLib.Internals
{
    internal class SharedMemServer : IDisposable
    {

        private static readonly IntPtr NoFileHandle = new IntPtr(-1);

        public const UInt16 MAPSIZE = 16384;

        // Handle to the shared memory
        private IntPtr hMem = new IntPtr(0);

        // Pointer to mapped shared memory
        public unsafe byte* pMem = null;

        // Name based on product name for shared objects.
        private string memName;


        ~SharedMemServer()
        {
            Close();
        }

        public unsafe bool Open(string strName)
        {
            // Close file if already open
            if (IsOpen())
            {
                Close();
            }
            memName = strName;
            memName += "_sm";

            Debug.Assert(hMem == IntPtr.Zero);
            Debug.Assert(pMem == null);

            // Try to open existing file
            if (!_Open())
            {
                // Create if none exist
                if (!_Create())
                {
                    return false;
                }
            }

            Debug.Assert(!(hMem == IntPtr.Zero));

            // Map the transport memory
            // Obtain a read/write map for the entire file
            pMem = (byte*)Kernel32.MapViewOfFile(hMem, FileRights.ReadWrite, 0, 0, 0);

            if (pMem == null)
            {
                Trace.WriteLine
                  ("Attempt to map shared memory failed: " + Marshal.GetLastWin32Error());
                Close();
                return false;
            }

            Trace.WriteLine("Shared memory endpoint created\n");
            return true;

        } 


        public unsafe void Close()
        {
            if (pMem != null)
            {
                Kernel32.UnmapViewOfFile((System.IntPtr)pMem);
                pMem = null;
            }

            // Close the transport mechanism
            if (hMem != IntPtr.Zero)
            {
                Kernel32.CloseHandle(hMem);
                hMem = IntPtr.Zero;
            }

        } 


        public bool IsOpen()
        {
            return !(hMem == IntPtr.Zero);
        }

        
        private bool _Create()
        {
            string strName = "";

            int nAttempt = 0;

            do
            {
                // First try the global namespace
                if (nAttempt == 0)
                    strName = "Global\\" + memName.ToString();

                // Next the the local namespace
                else if (nAttempt == 1)
                    strName = "Local\\" + memName.ToString();

                hMem = Kernel32.CreateFileMapping(NoFileHandle, 0,
                                                FileProtection.ReadWrite,
                                                0, MAPSIZE, strName);
                if (hMem == IntPtr.Zero)
                    throw new Exception
                      ("Open/create error: " + Marshal.GetLastWin32Error());


            } while ((hMem == IntPtr.Zero) && ++nAttempt < 2);

            if (hMem == IntPtr.Zero)
            {
                Trace.WriteLine("Failed to create shared memory object!");
                return false;
            }

            Trace.WriteLine("Shared memory object created as " + strName);
            return true;
        } 


        private bool _Open()
        {
            int nSession = -2;
            string strName = "";

            do
            {
                // Try the global table ( is a service or at least running with admin rights)
                if (nSession == -2)
                    strName = "Global\\" + memName.ToString();

                // Try the local session (runtime is in the same user session)
                else if (nSession == -1)
                    strName = "Local\\" + memName.ToString();

                // Try session-specific (admin and runtime are in different local user sessions)
                else
                    strName = "Session\\" + nSession.ToString() + memName.ToString();

                hMem = Kernel32.OpenFileMapping(FileRights.ReadWrite, false, strName);

            } while ((hMem == IntPtr.Zero) && nSession++ < 255);

            if (hMem != IntPtr.Zero)
            {
                Trace.WriteLine("Shared memory opened as {0}", strName);
                return true;
            }

            return false;
        } 

        public unsafe IntPtr Root
        {
            get
            {
                return ((System.IntPtr)pMem);
            }
        }


        public unsafe void Dispose()
        {
            if (pMem != null)
            {
                Kernel32.UnmapViewOfFile((System.IntPtr)pMem);
            }

            if (hMem != IntPtr.Zero)
            {
                Kernel32.CloseHandle(hMem);
            }
            pMem = null;
            hMem = IntPtr.Zero;
        }

    }

}