using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class StringTag : ValueTagBase<string>
    {

        public StringTag(string name, AccessType accessType, string description, string groupName, int maxLength)
            : base(name, (ushort) maxLength, 0, 0, ValueTypes.T_STRING, accessType, description, groupName)
        {

        }

        public StringTag(string name, string description, int maxLength)
            : this(name, AccessType.READWRITE, description, "", maxLength)
        {

        }

        protected override void SaveValueToCidTag(string value)
        {
            if (value == null)
            {
                value = "";
            }

            if (value.Length > base.CidTag.tagReadData.value.valueStringSize)
            {
                throw new InvalidOperationException("String length too long for tag");
            }
            else
            {
                base.CidTag.tagReadData.value.valueString = value;
            }
        }

        protected override string GetValueFromCidTag()
        {
            return base.CidTag.tagReadData.value.valueString;
        }
    }
}
