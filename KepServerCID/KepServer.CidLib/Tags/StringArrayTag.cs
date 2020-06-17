using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class StringArrayTag : SimpleTypeArrayTagBase<string>
    {

        public StringArrayTag(string name, int columns, AccessType accessType, string description, string groupName, int maxStringLength)
            : base(name, (ushort) maxStringLength, 1, (ushort)columns, ValueTypes.T_STRING | ValueTypes.T_ARRAY, accessType, description, groupName)
        {

        }

        public StringArrayTag(string name, int columns, string description, int maxStringLength)
            : this(name, columns, AccessType.READWRITE, description, "", maxStringLength)
        {
            
        }

    }
}
