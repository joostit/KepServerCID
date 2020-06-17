using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class FloatTag : ValueTagBase<float>
    {

        public FloatTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_FLOAT, accessType, description, groupName)
        {

        }

        public FloatTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

        protected override void SaveValueToCidTag(float value)
        {
            base.CidTag.tagReadData.value.valueFloat = value;
        }

        protected override float GetValueFromCidTag()
        {
            return base.CidTag.tagReadData.value.valueFloat;
        }
    }
}
