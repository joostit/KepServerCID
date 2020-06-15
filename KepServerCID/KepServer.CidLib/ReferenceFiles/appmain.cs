// **************************************************************************
// File:  appmain.cs
// Created:  12/03/2009 Copyright (c) 2009
//
// Description: Main entry point. Creates a configuration name based on the
// the application name. Calls the MemInterface class to initialize shared memory
// or initiate configuration export based on command line argument.
//
// **************************************************************************
// class library references
using System;

// common namespace for related files in project
namespace CidaRefImplCsharp
{

    // This application uses a pointer to unmanaged (shared) memory.
    // To build, you must enable "Allow unsafe code" in project build properties.
    public class CidConnector
    {
        public void Run(string[] args)
        {
            bool exportConfig = false;
            string strConfigName = "";
            string strApplicationDir = "";

            // Get application info for naming the shared memory file and mutex
            GetConfigInfo(ref strConfigName, ref strApplicationDir, args, ref exportConfig);

            // Start the interface to shared memory
            MemInterface.Start(args, strConfigName, strApplicationDir, exportConfig);

        } //Main ()

        // *************************************************************************************
        private void GetConfigInfo(ref string strConfigName, ref string strApplicationDir, string[] args, ref bool exportConfig)
        {

            // Parse command line
            if (args.Length > 0)
            {

                // Export Configuration
                if (args[0] == "-exportconfig")
                {
                    exportConfig = true;
                }

                else
                {
                    string msg = "Invalid argument provided. Usage: " +
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                        " [-exportconfig]";
                    Console.WriteLine(msg);
                }
            }

            // Important: The following will generate a Configuration Name based on the application's binary file name.
            // This is not a requirement and can be anything the vendor desires.

            // Get the application's directory
            string strTempApplicationDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            // Trim the leading chars 'file:\'
            char[] TrimChar = { 'f', 'i', 'l', 'e', ':', '\\' };
            strApplicationDir = strTempApplicationDir.TrimStart(TrimChar);

            // Get the application's name without the .exe extension
            strConfigName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        } // GetConfigInfo ()

    } // class AppMain

} //namespace CidaRefImplCsharp
