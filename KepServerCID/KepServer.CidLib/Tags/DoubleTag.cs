using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class DoubleTag : TagApiBase
    {
        public double Value
        {
            get
            {
                if (CidTag != null)
                {
                    return base.CidTag.tagReadData.value.valueDouble;
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
                    base.CidTag.tagReadData.value.valueDouble = value;
                }
            }
        }

        public DoubleTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_DOUBLE, accessType, description, groupName)
        {

        }

        public DoubleTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

    }
}
