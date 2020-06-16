using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class DateTag : TagApiBase
    {

        public DateTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_DATE, accessType, description, groupName)
        {

        }

        public DateTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
