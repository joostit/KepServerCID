using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class WordTag : ValueTagBase<ushort>
    {

        public WordTag(string name, AccessType accessType ,string description, string groupName)
            :base(name, 0, 0, 0, ValueTypes.T_WORD, accessType, description, groupName)
        {

        }

        public WordTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

        protected override void SaveValueToCidTag(ushort value)
        {
            base.CidTag.tagReadData.value.valueWord = value;
        }

        protected override ushort GetValueFromCidTag()
        {
            return base.CidTag.tagReadData.value.valueWord;
        }
    }
}
