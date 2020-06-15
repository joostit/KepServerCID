using CidaRefImplCsharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Types
{
    // *************************************************************************************
    public struct DeviceEntry
    {
        public string strName;
        public string strID;
        public List<TagEntry> tagEntryList;

        // *************************************************************************************
        public DeviceEntry(string _strName, string _strID)
        {
            strName = _strName;
            strID = _strID;
            tagEntryList = null;
        }

    } // public struct DEVICEENTRY 
}
