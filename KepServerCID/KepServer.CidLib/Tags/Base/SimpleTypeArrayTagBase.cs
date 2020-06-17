using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags.Base
{
    public abstract class SimpleTypeArrayTagBase<TDataType> : ArrayTagBase<TDataType, TDataType>
    {
        protected SimpleTypeArrayTagBase(string name, ushort stringSize, ushort arrayRows, ushort arrayCols, ushort valueType, AccessType access, string description, string groupName) 
            : base(name, stringSize, arrayRows, arrayCols, valueType, access, description, groupName)
        {
        }

        protected override TDataType CidToNetType(TDataType value)
        {
            return value;
        }

        protected override TDataType NetToCidType(TDataType value)
        {
            return value;
        }
    }
}
