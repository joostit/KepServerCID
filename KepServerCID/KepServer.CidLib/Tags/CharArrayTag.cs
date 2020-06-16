using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class CharArrayTag : TagApiBase
    {

        public CharArrayTag(string name, int rows, int columns, AccessType accessType, string description, string groupName)
            : base(name, 0, (ushort)rows, (ushort)columns, ValueTypes.T_CHAR | ValueTypes.T_ARRAY, accessType, description, groupName)
        {

        }

        public CharArrayTag(string name, int rows, int columns, string description)
            : this(name, rows, columns, AccessType.READWRITE, description, "")
        {

        }
    }
}
