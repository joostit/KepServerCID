using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class DoubleTag : TagDefinition
    {

        public DoubleTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, Value.T_DOUBLE, accessType, description, groupName)
        {

        }

        public DoubleTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
