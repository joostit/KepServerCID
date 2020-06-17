using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class DateTag : ValueTagBase<DateTime>
    {

        public DateTag(string name, AccessType accessType, string description, string groupName)
            : base(name, 0, 0, 0, ValueTypes.T_DATE, accessType, description, groupName)
        {

        }

        public DateTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }

        protected override void SaveValueToCidTag(DateTime value)
        {
            base.CidTag.tagReadData.value.valueDate = value.ToOADate();
        }

        protected override DateTime GetValueFromCidTag()
        {
            return DateTime.FromOADate(base.CidTag.tagReadData.value.valueDate);
        }
    }
}
