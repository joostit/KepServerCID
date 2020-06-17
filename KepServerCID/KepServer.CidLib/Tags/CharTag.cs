using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class CharTag : ValueTagBase<sbyte>
    {

        public CharTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_CHAR, accessType, description, groupName)
        {

        }

        public CharTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

        protected override void SaveValueToCidTag(sbyte value)
        {
            base.CidTag.tagReadData.value.valueChar = value;
        }

        protected override sbyte GetValueFromCidTag()
        {
            return base.CidTag.tagReadData.value.valueChar;
        }
    }
}
