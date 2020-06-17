using System;
using System.Collections.Generic;
using System.Text;
using WORD = System.UInt16;
using DWORD = System.UInt32;
using VALTYPE = System.UInt16;
using KepServer.CidLib.Types;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using KepServer.CidLib.Internals;

namespace KepServer.CidLib.Tags.Base
{
    public abstract class TagBase
    {
        public event EventHandler NewDataAvailable;

        internal Tag CidTag { get; set; }

        public string Name { get; set; }

        internal WORD StringSize { get; set; }

        internal WORD ArrayRows { get; set; }

        internal WORD ArrayCols { get; set; }

        internal VALTYPE ValueType { get; set; } 

        internal AccessType Access { get; set; }
        
        internal string Description { get; set; }

        internal string GroupName { get; set; }

        public TagBase(string name, WORD stringSize, WORD arrayRows,
            WORD arrayCols, VALTYPE valueType, AccessType access,
            string description, string groupName)
        {
            this.Name = name;
            this.StringSize = stringSize;
            this.ArrayRows = arrayRows;
            this.ArrayCols = arrayCols;
            this.ValueType = valueType;
            this.Access = access;
            this.Description = description;
            this.GroupName = groupName;
        }
       

        internal TagEntry ToTagEntry()
        {
            return new TagEntry(Name, StringSize, ArrayRows, ArrayCols, ValueType, Access, Description, GroupName);
        }

        internal void SetInternalTag(Tag cidTag)
        {
            CidTag = cidTag;
            CidTag.NewDataAvailable += (s, e) => OnNewDataAvailable();
        }

        protected virtual void OnNewDataAvailable()
        {
            NewDataAvailable.Invoke(this, EventArgs.Empty);
        }
    }
}
