// **************************************************************************
// Created:  11/30/2009 Copyright (c) 2009
//
// **************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using WORD = System.UInt16;
using DWORD = System.UInt32;
using VALTYPE = System.UInt16;
using BOOL = System.UInt16;
using LONG = System.Int32;
using KepServer.CidLib.Types;

namespace KepServer.CidLib.Internals
{
    /// <summary>
    /// A class which encapsulates functionality
    /// pertaining to a tag value.
    /// </summary>
    public class Value
    {


        public const int TYPE_OFFSET = 0;
        public const int VALUE_OFFSET = 4;
        public const int EXTSIZE_OFFSET = 12;
        public const int EXTVALUE_OFFSET = 14;

        public VALTYPE valueType = ValueTypes.T_UNDEFINED;
        public byte[] value8ByteArray = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //always 8
        public WORD valueExtSize;

        // The Extended Byte Array and Dynamic Array are instantiated
        // only for strings and array types.
        public byte[] valueExtByteArray; // used for stream reading and writing
        public Array valueDynamicArray; // created for array types
        public WORD valueStringSize;

        // These are the local storage for all possible data types
        // The appropriate one will be loaded into the 8-byte value array
        internal BOOL valueBool { get; set; }
        internal byte valueByte { get; set; }
        internal sbyte valueChar { get; set; }
        internal WORD valueWord { get; set; }
        internal WORD valueShort { get; set; }
        internal DWORD valueDword { get; set; }
        internal LONG valueLong { get; set; }
        internal float valueFloat { get; set; }
        internal double valueDouble { get; set; }
        internal double valueDate { get; set; } = DateTime.Now.ToOADate();
        internal string valueString { get; set; } = ""; //default value is zero



        public Value(VALTYPE vType, WORD stringSize, int nRows, int nCols)
        {

            if (vType == ValueTypes.T_UNDEFINED)
            {
                Debug.Assert(false);
                return;
            }
            valueType = vType;
            valueStringSize = stringSize;

            if ((vType >= ValueTypes.T_ARRAY) || (vType == ValueTypes.T_STRING))
            {

                //create the required arrays
                if ((vType == ValueTypes.T_STRING) && (vType != ValueTypes.T_ARRAY))
                {
                    valueDynamicArray = new char[stringSize]; // unicode by default
                    valueExtSize = (ushort)(valueDynamicArray.Length * sizeof(char)); //size in bytes
                }

                else
                {
                    int[] aLen = new int[2] { nRows, nCols };
                    valueDynamicArray = Array.CreateInstance(GetSysType(vType), aLen);

                    if (vType == (ValueTypes.T_ARRAY | ValueTypes.T_STRING))
                    {

                        // for string (object) arrays we create a new string of (stringsize) length at each location                        
                        string tmpStr = new string(' ', this.valueStringSize);

                        for (int i = this.valueDynamicArray.GetLowerBound(0); i <= this.valueDynamicArray.GetUpperBound(0); i++)
                        {

                            for (int j = this.valueDynamicArray.GetLowerBound(1); j <= this.valueDynamicArray.GetUpperBound(1); j++)
                            {
                                this.valueDynamicArray.SetValue(tmpStr.Substring(0, this.valueStringSize), i, j);
                            }
                        }
                        valueExtSize = (WORD)((valueDynamicArray.Length * sizeof(char) * stringSize) + sizeof(WORD));//add sizeof (stringSize)
                    }

                    else
                    {
                        valueExtSize = (WORD)(valueDynamicArray.Length * SizeOf(vType));
                    }
                }

                // create the array that will hold bytes for shared memory access
                valueExtByteArray = new byte[valueExtSize];
            }
            else
            {
                valueExtSize = 0;
            }

        } 



        public VALTYPE GetValueType()
        {
            return (valueType);
        }


        /// <summary>
        /// Returns the system-defined Type for value types and objects. This is
        /// used to pass the tag value Type to function Array.CreateInstance () when
        /// dynamically creating tag arrays.
        /// </summary>
        /// <param name="vType"></param>
        /// <returns></returns>
        private Type GetSysType(VALTYPE vType)
        {

            switch (vType & ~ValueTypes.T_ARRAY)
            {
                case ValueTypes.T_UNDEFINED:
                    return typeof(int);
                case ValueTypes.T_BOOL:
                case ValueTypes.T_SHORT:
                case ValueTypes.T_WORD:
                    return typeof(WORD);
                case ValueTypes.T_BYTE:
                    return typeof(byte);
                case ValueTypes.T_CHAR:
                    return typeof(sbyte);
                case ValueTypes.T_LONG:
                    return typeof(LONG);
                case ValueTypes.T_DWORD:
                    return typeof(DWORD);
                case ValueTypes.T_FLOAT:
                    return typeof(float);
                case ValueTypes.T_DOUBLE:
                    return typeof(double);
                case ValueTypes.T_DATE:
                    return typeof(double);
                case ValueTypes.T_STRING:
                    return typeof(string);
                default:
                    return (typeof(int));
            }

        }


        public static int SizeOf(VALTYPE dataType)
        {
            dataType = (VALTYPE)(dataType & ~ValueTypes.T_ARRAY);

            switch (dataType)
            {
                case ValueTypes.T_BYTE:
                case ValueTypes.T_CHAR:
                    return (sizeof(byte));

                case ValueTypes.T_SHORT:
                case ValueTypes.T_WORD:
                case ValueTypes.T_BOOL:
                    return sizeof(WORD);

                case ValueTypes.T_LONG:
                case ValueTypes.T_DWORD:
                    return sizeof(DWORD);

                case ValueTypes.T_FLOAT:
                    return sizeof(float);

                case ValueTypes.T_DOUBLE:
                case ValueTypes.T_DATE:
                    return sizeof(double);

                default:
                    return (0);
            }
        }


        public static bool IsValidValueType(VALTYPE tType)
        {
            if ((tType & ValueTypes.T_ARRAY) == ValueTypes.T_ARRAY) // array bit set?
            {
                tType = (VALTYPE)(tType & ~ValueTypes.T_ARRAY); //strip array bit, leave type bit
            }

            switch (tType)
            {
                case ValueTypes.T_BOOL:
                case ValueTypes.T_BYTE:
                case ValueTypes.T_CHAR:
                case ValueTypes.T_WORD:
                case ValueTypes.T_SHORT:
                case ValueTypes.T_DWORD:
                case ValueTypes.T_LONG:
                case ValueTypes.T_FLOAT:
                case ValueTypes.T_DOUBLE:
                case ValueTypes.T_DATE:
                case ValueTypes.T_STRING:
                    return (true);

                default:
                    return (false);
            }
        }


        /// <summary>
        /// Changes the value of a tag for demonstration purposes.
        /// </summary>
        public void Increment()
        {
            switch (this.valueType)
            {

                case ValueTypes.T_BOOL:
                    this.valueBool = (WORD)((int)this.valueBool > 0 ? 0 : 1);
                    break;

                case ValueTypes.T_BYTE:
                    this.valueByte++;
                    break;

                case ValueTypes.T_CHAR:
                    this.valueChar++;
                    break;

                case ValueTypes.T_WORD:
                    this.valueWord++;
                    break;

                case ValueTypes.T_SHORT:
                    this.valueShort++;
                    break;

                case ValueTypes.T_DWORD:
                    this.valueDword++;
                    break;

                case ValueTypes.T_LONG:
                    this.valueLong++;
                    break;

                case ValueTypes.T_FLOAT:
                    this.valueFloat++;
                    break;

                case ValueTypes.T_DOUBLE:
                    this.valueDouble++;
                    break;

                case ValueTypes.T_DATE:
                    this.valueDate = DateTime.Now.ToOADate();
                    break;

                case ValueTypes.T_STRING:
                    //Demo string from current time seconds.
                    //DateTime newSysNow = DateTime.Now;
                    //string tmpStr = "Seconds " + newSysNow.Second.ToString () + "." + newSysNow.Millisecond.ToString () + "      "; //padding
                    //this.valueString = tmpStr.Substring (0, tmpStr.Length > this.valueStringSize ? this.valueStringSize : tmpStr.Length);
                    //throw new NotSupportedException("Incrementing a string is not supported");


                default:
                    if ((WORD)(this.valueType & ValueTypes.T_ARRAY) == ValueTypes.T_ARRAY)
                    {
                        VALTYPE type = (VALTYPE)(this.valueType & ~ValueTypes.T_ARRAY);

                        for (int i = this.valueDynamicArray.GetLowerBound(0); i <= this.valueDynamicArray.GetUpperBound(0); i++)
                        {
                            for (int j = this.valueDynamicArray.GetLowerBound(1); j <= this.valueDynamicArray.GetUpperBound(1); j++)
                            {
                                switch (type)
                                {
                                    case ValueTypes.T_BOOL:
                                        {
                                            WORD aVal = (WORD)this.valueDynamicArray.GetValue(i, j);
                                            aVal = (WORD)(aVal > 0 ? 0 : 1); ;
                                            this.valueDynamicArray.SetValue(aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_BYTE:
                                        {
                                            byte aVal = (byte)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_CHAR:
                                        {
                                            sbyte aVal = (sbyte)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_WORD:
                                    case ValueTypes.T_SHORT:
                                        {
                                            ushort aVal = (ushort)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_DWORD:
                                        {
                                            DWORD aVal = (DWORD)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_LONG:
                                        {
                                            LONG aVal = (LONG)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_FLOAT:
                                        {
                                            float aVal = (float)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_DOUBLE:
                                        {
                                            double aVal = (double)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_DATE:
                                        {
                                            double aVal = DateTime.Now.ToOADate();
                                            this.valueDynamicArray.SetValue(aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_STRING:
                                        {
                                            break;
                                        }
                                }
                            } 
                        }
                    }
                    else
                    {
                        break;
                    }
                    break;
            } 

        }


        public void SetArrayValues(Array values)
        {
            Array.Copy(values, valueDynamicArray, values.Length);
        }

        public Array GetArray()
        {
            return valueDynamicArray;
        }

        public void SetArrayElement(object value, int row, int column)
        {
            valueDynamicArray.SetValue(value, row, column);
        }

        /// <summary>
        /// Translates the tag data value into a byte array that may be written to
        /// the shared memory stream. Returns an 8-byte array reference for basic types
        /// and a dynamically-sized byte array reference for strings and array types.
        /// </summary>
        /// <returns></returns>
        public byte[] GetArrayFromValue()
        {
            byte[] a8byte = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            byte[] aTmp = new byte[8];

            byte[] aExt = new byte[this.valueExtSize]; //for strings & arrays

            switch (this.valueType)
            {
                case ValueTypes.T_BOOL:
                    aTmp = BitConverter.GetBytes(this.valueBool);
                    break;

                case ValueTypes.T_BYTE:
                    aTmp = BitConverter.GetBytes(this.valueByte);
                    break;

                case ValueTypes.T_CHAR:
                    aTmp = BitConverter.GetBytes(this.valueChar);
                    break;

                case ValueTypes.T_WORD:
                    aTmp = BitConverter.GetBytes(this.valueWord);
                    break;

                case ValueTypes.T_SHORT:
                    aTmp = BitConverter.GetBytes(this.valueShort);
                    break;

                case ValueTypes.T_DWORD:
                    aTmp = BitConverter.GetBytes(this.valueDword);
                    break;

                case ValueTypes.T_LONG:
                    aTmp = BitConverter.GetBytes(this.valueLong);
                    break;

                case ValueTypes.T_FLOAT:
                    aTmp = BitConverter.GetBytes(this.valueFloat);
                    break;

                case ValueTypes.T_DOUBLE:
                    aTmp = BitConverter.GetBytes(this.valueDouble);
                    break;

                case ValueTypes.T_DATE:
                    aTmp = BitConverter.GetBytes(this.valueDate);
                    break;

                case ValueTypes.T_STRING:
                    aExt = System.Text.Encoding.Unicode.GetBytes(this.valueString);
                    break;

                default:
                    if ((WORD)(this.valueType & ValueTypes.T_ARRAY) == ValueTypes.T_ARRAY)
                    {
                        VALTYPE type = (VALTYPE)(this.valueType & ~ValueTypes.T_ARRAY);
                        switch (type)
                        {
                            case ValueTypes.T_BOOL:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(BOOL));
                                    break;
                                }
                            case ValueTypes.T_BYTE:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(byte));
                                    break;
                                }
                            case ValueTypes.T_CHAR:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(sbyte));
                                    break;
                                }
                            case ValueTypes.T_WORD:
                            case ValueTypes.T_SHORT:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(ushort));
                                    break;
                                }
                            case ValueTypes.T_DWORD:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(DWORD));
                                    break;
                                }
                            case ValueTypes.T_LONG:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(LONG));
                                    break;
                                }
                            case ValueTypes.T_FLOAT:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(float));
                                    break;
                                }
                            case ValueTypes.T_DOUBLE:
                            case ValueTypes.T_DATE:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(double));
                                    break;
                                }
                            case ValueTypes.T_STRING:
                                {
                                    byte[] abStr = new byte[this.valueStringSize * sizeof(char)];
                                    for (int i = this.valueDynamicArray.GetLowerBound(0); i <= this.valueDynamicArray.GetUpperBound(0); i++)
                                    {
                                        for (int j = this.valueDynamicArray.GetLowerBound(1); j <= this.valueDynamicArray.GetUpperBound(1); j++)
                                        {
                                            string strVal = (string)this.valueDynamicArray.GetValue(i, j);
                                            abStr = System.Text.Encoding.Unicode.GetBytes(strVal);
                                            Array.Copy(abStr, 0, aExt, (i + 1) * j * valueStringSize * sizeof(char), strVal.Length * sizeof(char));
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    else
                    {
                        break;
                    }
                    break;
            }
            if (aExt.Length != 0)
            {
                return (aExt);
            }
            else
            {
                Array.Copy(aTmp, a8byte, aTmp.Length);
                return (a8byte);
            }
        }

        /// <summary>
        /// Converts the byte array read from shared memory to the appropriate tag
        /// value.
        /// </summary>
        public void SetValueFromArray()
        {
            byte[] a8byte = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            Array.Copy(this.value8ByteArray, a8byte, this.value8ByteArray.Length);

            switch (this.valueType)
            {
                case ValueTypes.T_BOOL:
                    this.valueBool = BitConverter.ToUInt16(a8byte, 0);
                    break;

                case ValueTypes.T_BYTE:
                    this.valueByte = (a8byte[0]);
                    break;

                case ValueTypes.T_CHAR:
                    this.valueChar = (sbyte)(a8byte[0]);
                    break;

                case ValueTypes.T_WORD:
                    this.valueWord = BitConverter.ToUInt16(a8byte, 0);
                    break;

                case ValueTypes.T_SHORT:
                    this.valueShort = BitConverter.ToUInt16(a8byte, 0);
                    break;

                case ValueTypes.T_DWORD:
                    this.valueDword = BitConverter.ToUInt32(a8byte, 0);
                    break;

                case ValueTypes.T_LONG:
                    this.valueLong = BitConverter.ToInt32(a8byte, 0);
                    break;

                case ValueTypes.T_FLOAT:
                    valueFloat = BitConverter.ToSingle(a8byte, 0);
                    break;

                case ValueTypes.T_DOUBLE:
                    this.valueDouble = BitConverter.ToDouble(a8byte, 0);
                    break;

                case ValueTypes.T_DATE:
                    this.valueDate = BitConverter.ToDouble(a8byte, 0);
                    //writing to date not currently supported
                    //this.valueDate = BitConverter.ToInt64 (a8byte, 0);
                    break;

                case ValueTypes.T_STRING:
                    {
                        string tmpString = System.Text.UnicodeEncoding.Unicode.GetString(this.valueExtByteArray);

                        // In some languages, such as C and C++, a null character indicates the end of a string. 
                        // In the .NET Framework, a null character can be embedded in a string. 
                        // Therefore, one must extract the NULL terminated string to remove garbage characters. 
                        int tmpIndex = tmpString.IndexOf((char)0);
                        if (tmpIndex > 0 && tmpIndex + 1 < tmpString.Length)
                        {
                            this.valueString = tmpString.Remove(tmpIndex + 1);
                        }
                        else
                        {
                            this.valueString = tmpString;
                        }
                        break;
                    }

                default:
                    if ((WORD)(this.valueType & ValueTypes.T_ARRAY) == ValueTypes.T_ARRAY)
                    {

                        int offset = 0;
                        for (int i = this.valueDynamicArray.GetLowerBound(0); i <= this.valueDynamicArray.GetUpperBound(0); i++)
                        {
                            for (int j = this.valueDynamicArray.GetLowerBound(1); j <= this.valueDynamicArray.GetUpperBound(1); j++)
                            {
                                VALTYPE type = (VALTYPE)(this.valueType & ~ValueTypes.T_ARRAY);
                                switch (type)
                                {
                                    case ValueTypes.T_BOOL:
                                        {
                                            byte[] aBits = new byte[sizeof(UInt16)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToUInt16(aBits, 0), i, j);
                                            offset += sizeof(UInt16);
                                            break;
                                        }
                                    case ValueTypes.T_BYTE:
                                        {
                                            byte aVal = this.valueExtByteArray[(i + 1) * j];
                                            this.valueDynamicArray.SetValue(aVal, i, j);
                                            break;
                                        }
                                    case ValueTypes.T_CHAR:
                                        {
                                            byte[] aBits = new byte[sizeof(byte)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue((sbyte)aBits[0], i, j);
                                            offset += sizeof(byte);
                                            break;
                                        }
                                    case ValueTypes.T_WORD:
                                        {
                                            byte[] aBits = new byte[sizeof(WORD)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToUInt16(aBits, 0), i, j);
                                            offset += sizeof(WORD);
                                            break;
                                        }
                                    case ValueTypes.T_SHORT:
                                        {
                                            byte[] aBits = new byte[sizeof(WORD)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToUInt16(aBits, 0), i, j);
                                            offset += sizeof(WORD);
                                            break;
                                        }
                                    case ValueTypes.T_DWORD:
                                        {
                                            byte[] aBits = new byte[sizeof(DWORD)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToUInt32(aBits, 0), i, j);
                                            offset += sizeof(DWORD);
                                            break;
                                        }
                                    case ValueTypes.T_LONG:
                                        {
                                            byte[] aBits = new byte[sizeof(Int32)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToInt32(aBits, 0), i, j);
                                            offset += sizeof(Int32);
                                            break;
                                        }
                                    case ValueTypes.T_FLOAT:
                                        {
                                            byte[] aBits = new byte[sizeof(Int32)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToSingle(aBits, 0), i, j);
                                            offset += sizeof(Int32);
                                            break;
                                        }
                                    case ValueTypes.T_DOUBLE:
                                        {
                                            byte[] aBits = new byte[sizeof(Int64)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToDouble(aBits, 0), i, j);
                                            offset += sizeof(Int64);
                                            break;
                                        }
                                    case ValueTypes.T_DATE:
                                        {
                                            //writing to date not currently supported
                                            //byte[] aBits = new byte[sizeof (Int64)];
                                            //Array.Copy (this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            //this.valueDynamicArray.SetValue (BitConverter.ToDouble (aBits, 0), i, j);
                                            //offset += sizeof (Int64);
                                            throw new NotSupportedException("writing to date not currently supported");
                                        }
                                    case ValueTypes.T_STRING:
                                        {
                                            string tmpString = System.Text.UnicodeEncoding.Unicode.GetString(this.valueExtByteArray, offset, this.valueStringSize * sizeof(char));

                                            // In some languages, such as C and C++, a null character indicates the end of a string. 
                                            // In the .NET Framework, a null character can be embedded in a string. 
                                            // Therefore, one must extract the NULL terminated string to remove garbage characters. 
                                            int tmpIndex = tmpString.IndexOf((char)0);
                                            if (tmpIndex > 0 && tmpIndex + 1 < tmpString.Length)
                                            {
                                                this.valueDynamicArray.SetValue(tmpString.Remove(tmpIndex + 1), i, j);
                                            }
                                            else
                                            {
                                                this.valueDynamicArray.SetValue(tmpString, i, j);
                                            }

                                            offset += this.valueStringSize * sizeof(char);
                                            break;
                                        }
                                }
                            }
                        }

                    }
                    else
                    {
                        break;
                    }
                    break;
            }
        }

    }

}
