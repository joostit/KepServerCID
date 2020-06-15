using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class StringArrayTag : TagDefinition
    {

        public StringArrayTag(string name, int columns, AccessType accessType, string description, string groupName)
            : base(name, 0, 1, (ushort)columns, Value.T_STRING | Value.T_ARRAY, accessType, description, groupName)
        {

        }

        public StringArrayTag(string name, int columns, string description)
            : this(name, columns, AccessType.READWRITE, description, "")
        {
            
        }
    }
}
