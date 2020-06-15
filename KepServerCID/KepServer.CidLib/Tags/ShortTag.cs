using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class ShortTag : TagDefinition
    {

        public ShortTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, Value.T_SHORT, accessType, description, groupName)
        {

        }

        public ShortTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
