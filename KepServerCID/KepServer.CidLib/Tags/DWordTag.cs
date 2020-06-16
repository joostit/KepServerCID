using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class DWordTag : ValueTag<uint>
    {
        public DWordTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_DWORD, accessType, description, groupName)
        {

        }

        public DWordTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

        protected override void SaveValueToCidTag(uint value)
        {
            base.CidTag.tagReadData.value.valueDword = value;
        }

        protected override uint GetValueFromCidTag()
        {
            return base.CidTag.tagReadData.value.valueDword;
        }
    }
}
