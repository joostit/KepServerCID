using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class ShortTag : ValueTagBase<ushort>
    {

        public ShortTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_SHORT, accessType, description, groupName)
        {

        }

        public ShortTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

        protected override void SaveValueToCidTag(ushort value)
        {
            base.CidTag.tagReadData.value.valueShort = value;
        }

        protected override ushort GetValueFromCidTag()
        {
            return base.CidTag.tagReadData.value.valueShort;
        }
    }
}
