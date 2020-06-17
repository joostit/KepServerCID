using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags.Base
{
    public abstract class ValueTagBase<TNETValue> : TagBase
    {

        public TNETValue Value
        {
            get
            {
                if (CidTag == null)
                {
                    return default(TNETValue);
                }
                else
                {
                    return GetValueFromCidTag();
                }
            }
            set
            {
                SaveValueToCidTag(value);
            }
        }

        protected ValueTagBase(string name, ushort stringSize, ushort arrayRows, ushort arrayCols, ushort valueType, AccessType access, string description, string groupName) 
            : base(name, stringSize, arrayRows, arrayCols, valueType, access, description, groupName)
        {
        }

        protected abstract void SaveValueToCidTag(TNETValue value);
        protected abstract TNETValue GetValueFromCidTag();

    }
}
