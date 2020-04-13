using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class StringEnum
    {
        private static List<StringEnum> _values = new List<StringEnum>();

        private readonly string _value;

        private StringEnum(string value)
        {
            _value = value;
            _values.Add(this);
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

        public override bool Equals(object obj)
        {
            return obj is StringEnum && _value == ((StringEnum)obj)._value;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
        public static bool operator ==(StringEnum left, StringEnum right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(StringEnum left, StringEnum right)
        {
            return left._value != right._value;
        }
    }
}
