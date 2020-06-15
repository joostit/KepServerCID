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

namespace CidaRefImplCsharp
{
    /// <summary>
    /// A class which encapsulates functionality
    /// pertaining to a tag value.
    /// </summary>
    public class Value
    {
        public const VALTYPE T_UNDEFINED = 0;
        public const VALTYPE T_BOOL = 1;
        public const VALTYPE T_BYTE = 2;
        public const VALTYPE T_CHAR = 3;
        public const VALTYPE T_WORD = 4;
        public const VALTYPE T_SHORT = 5;
        public const VALTYPE T_DWORD = 6;
        public const VALTYPE T_LONG = 7;
        public const VALTYPE T_FLOAT = 8;
        public const VALTYPE T_DOUBLE = 9;
        public const VALTYPE T_DATE = 10;
        public const VALTYPE T_STRING = 11;
        public const VALTYPE T_ARRAY = 0x1000;

        public const int TYPE_OFFSET = 0;
        public const int VALUE_OFFSET = 4;
        public const int EXTSIZE_OFFSET = 12;
        public const int EXTVALUE_OFFSET = 14;

        public VALTYPE valueType = T_UNDEFINED;
        public byte[] value8ByteArray = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //always 8
        public WORD valueExtSize;

        // The Extended Byte Array and Dynamic Array are instantiated
        // only for strings and array types.
        public byte[] valueExtByteArray; // used for stream reading and writing
        public Array valueDynamicArray; // created for array types
        public WORD valueStringSize;

        // These are the local storage for all possible data types
        // The appropriate one will be loaded into the 8-byte value array
        private BOOL valueBool;
        private byte valueByte;
        private sbyte valueChar;
        private WORD valueWord;
        private WORD valueShort;
        private DWORD valueDword;
        private LONG valueLong;
        private float valueFloat;
        private double valueDouble;
        private double valueDate = DateTime.Now.ToOADate();
        private string valueString = ""; //default value is zero



        public Value(VALTYPE vType, WORD stringSize, int nRows, int nCols)
        {

            if (vType == T_UNDEFINED)
            {
                Debug.Assert(false);
                return;
            }
            valueType = vType;
            valueStringSize = stringSize;

            if ((vType >= Value.T_ARRAY) || (vType == Value.T_STRING))
            {

                //create the required arrays
                if ((vType == Value.T_STRING) && (vType != Value.T_ARRAY))
                {
                    valueDynamicArray = new char[stringSize]; // unicode by default
                    valueExtSize = (ushort)(valueDynamicArray.Length * sizeof(char)); //size in bytes
                }

                else
                {
                    int[] aLen = new int[2] { nRows, nCols };
                    valueDynamicArray = Array.CreateInstance(GetSysType(vType), aLen);

                    if (vType == (T_ARRAY | T_STRING))
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

            switch (vType & ~Value.T_ARRAY)
            {
                case T_UNDEFINED:
                    return typeof(int);
                case T_BOOL:
                case T_SHORT:
                case T_WORD:
                    return typeof(WORD);
                case T_BYTE:
                    return typeof(byte);
                case T_CHAR:
                    return typeof(sbyte);
                case T_LONG:
                    return typeof(LONG);
                case T_DWORD:
                    return typeof(DWORD);
                case T_FLOAT:
                    return typeof(float);
                case T_DOUBLE:
                    return typeof(double);
                case T_DATE:
                    return typeof(double);
                case T_STRING:
                    return typeof(string);
                default:
                    return (typeof(int));
            }

        }


        public static int SizeOf(VALTYPE dataType)
        {
            dataType = (VALTYPE)(dataType & ~Value.T_ARRAY);

            switch (dataType)
            {
                case Value.T_BYTE:
                case Value.T_CHAR:
                    return (sizeof(byte));

                case Value.T_SHORT:
                case Value.T_WORD:
                case Value.T_BOOL:
                    return sizeof(WORD);

                case Value.T_LONG:
                case Value.T_DWORD:
                    return sizeof(DWORD);

                case Value.T_FLOAT:
                    return sizeof(float);

                case Value.T_DOUBLE:
                case Value.T_DATE:
                    return sizeof(double);

                default:
                    return (0);
            }
        }


        public static bool IsValidValueType(VALTYPE tType)
        {
            if ((tType & T_ARRAY) == T_ARRAY) // array bit set?
            {
                tType = (VALTYPE)(tType & ~T_ARRAY); //strip array bit, leave type bit
            }

            switch (tType)
            {
                case T_BOOL:
                case T_BYTE:
                case T_CHAR:
                case T_WORD:
                case T_SHORT:
                case T_DWORD:
                case T_LONG:
                case T_FLOAT:
                case T_DOUBLE:
                case T_DATE:
                case T_STRING:
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

                case T_BOOL:
                    this.valueBool = (WORD)((int)this.valueBool > 0 ? 0 : 1);
                    break;

                case T_BYTE:
                    this.valueByte++;
                    break;

                case T_CHAR:
                    this.valueChar++;
                    break;

                case T_WORD:
                    this.valueWord++;
                    break;

                case T_SHORT:
                    this.valueShort++;
                    break;

                case T_DWORD:
                    this.valueDword++;
                    break;

                case T_LONG:
                    this.valueLong++;
                    break;

                case T_FLOAT:
                    this.valueFloat++;
                    break;

                case T_DOUBLE:
                    this.valueDouble++;
                    break;

                case T_DATE:
                    this.valueDate = DateTime.Now.ToOADate();
                    break;

                case T_STRING:
                    //Demo string from current time seconds.
                    //DateTime newSysNow = DateTime.Now;
                    //string tmpStr = "Seconds " + newSysNow.Second.ToString () + "." + newSysNow.Millisecond.ToString () + "      "; //padding
                    //this.valueString = tmpStr.Substring (0, tmpStr.Length > this.valueStringSize ? this.valueStringSize : tmpStr.Length);
                    //throw new NotSupportedException("Incrementing a string is not supported");


                default:
                    if ((WORD)(this.valueType & T_ARRAY) == T_ARRAY)
                    {
                        VALTYPE type = (VALTYPE)(this.valueType & ~T_ARRAY);

                        for (int i = this.valueDynamicArray.GetLowerBound(0); i <= this.valueDynamicArray.GetUpperBound(0); i++)
                        {
                            for (int j = this.valueDynamicArray.GetLowerBound(1); j <= this.valueDynamicArray.GetUpperBound(1); j++)
                            {
                                switch (type)
                                {
                                    case T_BOOL:
                                        {
                                            WORD aVal = (WORD)this.valueDynamicArray.GetValue(i, j);
                                            aVal = (WORD)(aVal > 0 ? 0 : 1); ;
                                            this.valueDynamicArray.SetValue(aVal, i, j);
                                            break;
                                        }
                                    case T_BYTE:
                                        {
                                            byte aVal = (byte)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case T_CHAR:
                                        {
                                            sbyte aVal = (sbyte)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case T_WORD:
                                    case T_SHORT:
                                        {
                                            ushort aVal = (ushort)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case T_DWORD:
                                        {
                                            DWORD aVal = (DWORD)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case T_LONG:
                                        {
                                            LONG aVal = (LONG)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case T_FLOAT:
                                        {
                                            float aVal = (float)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case T_DOUBLE:
                                        {
                                            double aVal = (double)this.valueDynamicArray.GetValue(i, j);
                                            this.valueDynamicArray.SetValue(++aVal, i, j);
                                            break;
                                        }
                                    case T_DATE:
                                        {
                                            double aVal = DateTime.Now.ToOADate();
                                            this.valueDynamicArray.SetValue(aVal, i, j);
                                            break;
                                        }
                                    case T_STRING:
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
                case T_BOOL:
                    aTmp = BitConverter.GetBytes(this.valueBool);
                    break;

                case T_BYTE:
                    aTmp = BitConverter.GetBytes(this.valueByte);
                    break;

                case T_CHAR:
                    aTmp = BitConverter.GetBytes(this.valueChar);
                    break;

                case T_WORD:
                    aTmp = BitConverter.GetBytes(this.valueWord);
                    break;

                case T_SHORT:
                    aTmp = BitConverter.GetBytes(this.valueShort);
                    break;

                case T_DWORD:
                    aTmp = BitConverter.GetBytes(this.valueDword);
                    break;

                case T_LONG:
                    aTmp = BitConverter.GetBytes(this.valueLong);
                    break;

                case T_FLOAT:
                    aTmp = BitConverter.GetBytes(this.valueFloat);
                    break;

                case T_DOUBLE:
                    aTmp = BitConverter.GetBytes(this.valueDouble);
                    break;

                case T_DATE:
                    aTmp = BitConverter.GetBytes(this.valueDate);
                    break;

                case T_STRING:
                    aExt = System.Text.Encoding.Unicode.GetBytes(this.valueString);
                    break;

                default:
                    if ((WORD)(this.valueType & T_ARRAY) == T_ARRAY)
                    {
                        VALTYPE type = (VALTYPE)(this.valueType & ~T_ARRAY);
                        switch (type)
                        {
                            case T_BOOL:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(BOOL));
                                    break;
                                }
                            case T_BYTE:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(byte));
                                    break;
                                }
                            case T_CHAR:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(sbyte));
                                    break;
                                }
                            case T_WORD:
                            case T_SHORT:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(ushort));
                                    break;
                                }
                            case T_DWORD:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(DWORD));
                                    break;
                                }
                            case T_LONG:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(LONG));
                                    break;
                                }
                            case T_FLOAT:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(float));
                                    break;
                                }
                            case T_DOUBLE:
                            case T_DATE:
                                {
                                    System.Buffer.BlockCopy(this.valueDynamicArray, 0, aExt, 0, this.valueDynamicArray.Length * sizeof(double));
                                    break;
                                }
                            case T_STRING:
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
                case T_BOOL:
                    this.valueBool = BitConverter.ToUInt16(a8byte, 0);
                    break;

                case T_BYTE:
                    this.valueByte = (a8byte[0]);
                    break;

                case T_CHAR:
                    this.valueChar = (sbyte)(a8byte[0]);
                    break;

                case T_WORD:
                    this.valueWord = BitConverter.ToUInt16(a8byte, 0);
                    break;

                case T_SHORT:
                    this.valueShort = BitConverter.ToUInt16(a8byte, 0);
                    break;

                case T_DWORD:
                    this.valueDword = BitConverter.ToUInt32(a8byte, 0);
                    break;

                case T_LONG:
                    this.valueLong = BitConverter.ToInt32(a8byte, 0);
                    break;

                case T_FLOAT:
                    valueFloat = BitConverter.ToSingle(a8byte, 0);
                    break;

                case T_DOUBLE:
                    this.valueDouble = BitConverter.ToDouble(a8byte, 0);
                    break;

                case T_DATE:
                    //writing to date not currently supported
                    //this.valueDate = BitConverter.ToInt64 (a8byte, 0);
                    throw new NotSupportedException("writing to date not currently supported");

                case T_STRING:
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
                    if ((WORD)(this.valueType & T_ARRAY) == T_ARRAY)
                    {

                        int offset = 0;
                        for (int i = this.valueDynamicArray.GetLowerBound(0); i <= this.valueDynamicArray.GetUpperBound(0); i++)
                        {
                            for (int j = this.valueDynamicArray.GetLowerBound(1); j <= this.valueDynamicArray.GetUpperBound(1); j++)
                            {
                                VALTYPE type = (VALTYPE)(this.valueType & ~T_ARRAY);
                                switch (type)
                                {
                                    case T_BOOL:
                                        {
                                            byte[] aBits = new byte[sizeof(UInt16)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToUInt16(aBits, 0), i, j);
                                            offset += sizeof(UInt16);
                                            break;
                                        }
                                    case T_BYTE:
                                        {
                                            byte aVal = this.valueExtByteArray[(i + 1) * j];
                                            this.valueDynamicArray.SetValue(aVal, i, j);
                                            break;
                                        }
                                    case T_CHAR:
                                        {
                                            byte[] aBits = new byte[sizeof(byte)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue((sbyte)aBits[0], i, j);
                                            offset += sizeof(byte);
                                            break;
                                        }
                                    case T_WORD:
                                        {
                                            byte[] aBits = new byte[sizeof(WORD)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToUInt16(aBits, 0), i, j);
                                            offset += sizeof(WORD);
                                            break;
                                        }
                                    case T_SHORT:
                                        {
                                            byte[] aBits = new byte[sizeof(WORD)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToUInt16(aBits, 0), i, j);
                                            offset += sizeof(WORD);
                                            break;
                                        }
                                    case T_DWORD:
                                        {
                                            byte[] aBits = new byte[sizeof(DWORD)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToUInt32(aBits, 0), i, j);
                                            offset += sizeof(DWORD);
                                            break;
                                        }
                                    case T_LONG:
                                        {
                                            byte[] aBits = new byte[sizeof(Int32)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToInt32(aBits, 0), i, j);
                                            offset += sizeof(Int32);
                                            break;
                                        }
                                    case T_FLOAT:
                                        {
                                            byte[] aBits = new byte[sizeof(Int32)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToSingle(aBits, 0), i, j);
                                            offset += sizeof(Int32);
                                            break;
                                        }
                                    case T_DOUBLE:
                                        {
                                            byte[] aBits = new byte[sizeof(Int64)];
                                            Array.Copy(this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            this.valueDynamicArray.SetValue(BitConverter.ToDouble(aBits, 0), i, j);
                                            offset += sizeof(Int64);
                                            break;
                                        }
                                    case T_DATE:
                                        {
                                            //writing to date not currently supported
                                            //byte[] aBits = new byte[sizeof (Int64)];
                                            //Array.Copy (this.valueExtByteArray, offset, aBits, 0, aBits.Length);
                                            //this.valueDynamicArray.SetValue (BitConverter.ToDouble (aBits, 0), i, j);
                                            //offset += sizeof (Int64);
                                            throw new NotSupportedException("writing to date not currently supported");
                                        }
                                    case T_STRING:
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
