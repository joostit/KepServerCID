using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class ShortTag : TagApiBase
    {

        public ushort Value
        {
            get
            {
                if (CidTag != null)
                {
                    return base.CidTag.tagReadData.value.valueShort;
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
                    base.CidTag.tagReadData.value.valueShort = value;
                }
            }
        }

        public ShortTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_SHORT, accessType, description, groupName)
        {

        }

        public ShortTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
