using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class FloatTag : TagDefinition
    {

        public FloatTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, Value.T_FLOAT, accessType, description, groupName)
        {

        }

        public FloatTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
