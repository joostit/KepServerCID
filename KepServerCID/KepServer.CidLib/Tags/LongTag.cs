using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class LongTag : TagApiBase
    {

        public LongTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_LONG, accessType, description, groupName)
        {

        }

        public LongTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
