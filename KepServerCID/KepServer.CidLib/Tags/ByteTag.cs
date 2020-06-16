using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class ByteTag : TagApiBase
    {
        public byte Value
        {
            get
            {
                if (CidTag != null)
                {
                    return base.CidTag.tagReadData.value.valueByte;
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
                    base.CidTag.tagReadData.value.valueByte = value;
                }
            }
        }

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
