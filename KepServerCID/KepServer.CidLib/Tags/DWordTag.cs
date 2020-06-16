using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class DWordTag : TagApiBase
    {

        public uint Value
        {
            get
            {
                if (CidTag != null)
                {
                    return base.CidTag.tagReadData.value.valueDword;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (CidTag != null)
                {
                    base.CidTag.tagReadData.value.valueDword = value;
                }
            }
        }

        public DWordTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_DWORD, accessType, description, groupName)
        {

        }

        public DWordTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
