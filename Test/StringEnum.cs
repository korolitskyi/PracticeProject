using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class StringEnum
    {
        private readonly string _value;

        private StringEnum(string value)
        {
            _value = value;
        }

        public static readonly StringEnum Test1 = new StringEnum("test1");

        public static implicit operator StringEnum(string value)
        {
            return new StringEnum(value);
        }

        public static implicit operator string(StringEnum value)
        {
            return value._value;
        }
    }
}
