using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class BoolTag : TagApiBase
    {

        public bool Value
        {
            get
            {
                if (CidTag != null)
                {
                    return Convert.ToBoolean(base.CidTag.tagReadData.value.valueBool);
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (CidTag != null)
                {
                    base.CidTag.tagReadData.value.valueBool = Convert.ToUInt16(value);
                }
            }
        }

        public BoolTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_BOOL, accessType, description, groupName)
        {

        }

        public BoolTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }
    }
}
