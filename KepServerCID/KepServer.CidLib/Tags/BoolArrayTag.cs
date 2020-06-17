using KepServer.CidLib.Internals;
using KepServer.CidLib.Tags.Base;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class BoolArrayTag : ArrayTagBase<bool, ushort>
    {

        public BoolArrayTag(string name, int rows, int columns, AccessType accessType, string description, string groupName)
            : base(name, 0, (ushort) rows, (ushort) columns, ValueTypes.T_BOOL | ValueTypes.T_ARRAY, accessType, description, groupName)
        {

        }

        public BoolArrayTag(string name, int rows, int columns, string description)
            : this(name, rows, columns, AccessType.READWRITE, description, "")
        {

        }


        protected override bool CidToNetType(ushort value)
        {
            return Convert.ToBoolean(value);
        }

        protected override ushort NetToCidType(bool value)
        {
            return Convert.ToUInt16(value);
        }

    }
}
