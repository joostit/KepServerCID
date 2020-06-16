using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class FloatTag : TagApiBase
    {

        public float Value
        {
            get
            {
                if (CidTag != null)
                {
                    return base.CidTag.tagReadData.value.valueFloat;
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
                    base.CidTag.tagReadData.value.valueFloat = value;
                }
            }
        }

        public FloatTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_FLOAT, accessType, description, groupName)
        {

        }

        public FloatTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
