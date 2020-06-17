using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class BoolTag : ValueTagBase<bool>
    {

        public BoolTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_BOOL, accessType, description, groupName)
        {

        }

        public BoolTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

        protected override void SaveValueToCidTag(bool value)
        {
            base.CidTag.tagReadData.value.valueBool = Convert.ToUInt16(value);
        }

        protected override bool GetValueFromCidTag()
        {
            return Convert.ToBoolean(base.CidTag.tagReadData.value.valueBool);
        }
    }
}
