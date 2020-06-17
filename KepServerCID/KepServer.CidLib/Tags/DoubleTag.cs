using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class DoubleTag : ValueTagBase<double>
    {
        public DoubleTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_DOUBLE, accessType, description, groupName)
        {

        }

        public DoubleTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

        protected override void SaveValueToCidTag(double value)
        {
            base.CidTag.tagReadData.value.valueDouble = value;
        }

        protected override double GetValueFromCidTag()
        {
            return base.CidTag.tagReadData.value.valueDouble;
        }
    }
}
