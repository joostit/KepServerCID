using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public abstract class ArrayTag : TagApiBase
    {


        public int Rows { get; private set; }

        public int Columns { get; private set; }


        protected ArrayTag(string name, ushort stringSize, ushort arrayRows, ushort arrayCols, ushort valueType, AccessType access, string description, string groupName) 
            : base(name, stringSize, arrayRows, arrayCols, valueType, access, description, groupName)
        {
            this.Rows = arrayRows;
            this.Columns = this.ArrayCols;
        }


    }
}
