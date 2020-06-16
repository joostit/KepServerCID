using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class ByteTag : TagApiBase
    {

        public ByteTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_BYTE, accessType, description, groupName)
        {

        }

        public ByteTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
