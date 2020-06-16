using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class StringTag : TagApiBase
    {

        public StringTag(string name, AccessType accessType, string description, string groupName, int maxLength)
            : base(name, (ushort) maxLength, 0, 0, ValueTypes.T_STRING, accessType, description, groupName)
        {

        }

        public StringTag(string name, string description, int maxLength)
            : this(name, AccessType.READWRITE, description, "", maxLength)
        {

        }

    }
}
