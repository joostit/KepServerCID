using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{

    /// <summary>
    /// Base class for array type tags. Note that each operation of getting and setting the Value property is relatively costly. For getting or setting individual 
    /// elements, use the index operator of this class.
    /// </summary>
    /// <typeparam name="TArrayType">The type of array</typeparam>
    /// <typeparam name="TNetElement">The type of element as used in c# .NET</typeparam>
    /// <typeparam name="TCidElement">The type of element as it is stored in the CID Tag</typeparam>
    public abstract class ArrayTag<TArrayType, TNetElement, TCidElement>
        : ValueTag<TArrayType>
    {

        public int Rows { get; private set; }

        public int Columns { get; private set; }

        public TNetElement this[int row, int column]
        {
            get
            {
                if(CidTag != null)
                {
                    TCidElement cidElement = (TCidElement) CidTag.tagReadData.value.GetArray().GetValue(row, column);
                    return CidToNetType(cidElement);
                }
                else
                {
                    return default(TNetElement);
                }
            }
            set
            {
                if (CidTag != null)
                {
                    TCidElement cidElement = NetToCidType(value);
                    CidTag.tagReadData.value.SetArrayElement(cidElement, row, column);
                }
            }
        }

        protected ArrayTag(string name, ushort stringSize, ushort arrayRows, ushort arrayCols, ushort valueType, AccessType access, string description, string groupName) 
            : base(name, stringSize, arrayRows, arrayCols, valueType, access, description, groupName)
        {
            this.Rows = arrayRows;
            this.Columns = this.ArrayCols;

            if (!typeof(Array).IsAssignableFrom(typeof(TArrayType)))
            {
                throw new InvalidCastException("TArrayType should be of a two dimensional array type");
            }
        }


        protected override TArrayType GetValueFromCidTag()
        {
            Array cidArray = CidTag.tagReadData.value.GetArray();
            TNetElement[,] retVal = new TNetElement[Rows, Columns];

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    TCidElement cidElement = (TCidElement) cidArray.GetValue(r, c);
                    TNetElement netElement = CidToNetType(cidElement);
                    retVal[r, c] = netElement;
                }
            }

            return (TArrayType) (object) retVal;
        }

        protected override void SaveValueToCidTag(TArrayType value)
        {
            Array targetArray = new TCidElement[Rows, Columns];
            Array sourceArray = value as Array;

            if (sourceArray == null) { throw new InvalidCastException("Cannot cast the input type to an array type"); }

            // Bounds check
            int sourceRows = sourceArray.GetUpperBound(0) - sourceArray.GetLowerBound(0) + 1;
            int sourceCols = sourceArray.GetUpperBound(1) - sourceArray.GetLowerBound(1) + 1;

            if(sourceCols != Columns || sourceRows != Rows)
            {
                throw new InvalidCastException("The provided value Row and/or Column count don't match this array tag's dimensions");
            }

            for(int r = 0; r < Rows; r++)
            {
                for(int c = 0; c < Columns; c++)
                {
                    TNetElement sourceElement = (TNetElement)sourceArray.GetValue(r, c);
                    TCidElement targetElement = NetToCidType(sourceElement);
                    targetArray.SetValue(targetElement, r, c);
                }
            }

            CidTag.tagReadData.value.SetArrayValues(targetArray);
        }


        protected abstract TNetElement CidToNetType(TCidElement value);

        protected abstract TCidElement NetToCidType(TNetElement value);

    }
}
