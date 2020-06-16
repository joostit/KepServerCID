using KepServer.CidLib.Internals;
using KepServer.CidLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace KepServer.CidLib.Tags
{
    public class WordTag : TagApiBase
    {

        public ushort Value
        {
            get
            {
                if (CidTag != null)
                {
                    return base.CidTag.tagReadData.value.valueWord;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (CidTag != null)
                {
                    base.CidTag.tagReadData.value.valueWord = value;
                }
            }
        }

        public WordTag(string name, AccessType accessType ,string description, string groupName)
            :base(name, 0, 0, 0, ValueTypes.T_WORD, accessType, description, groupName)
        {

        }

        public WordTag(string name, string description)
            : this(name, AccessType.READWRITE, description, "")
        {

        }


    }
}
