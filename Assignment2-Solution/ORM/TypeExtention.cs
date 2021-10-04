using System;
using System.Collections.Generic;
using System.Text;

namespace ORM
{
    public static class TypeExtention
    {
        public static bool IsSimpleType(this Type type)
        {
            return (type == typeof(string)
                || type == typeof(int)
                || type == typeof(double)
                || type == typeof(decimal)
                || type == typeof(bool)
                || type == typeof(DateTime)
                || type == typeof(float)
                || type == typeof(short)
                || type == typeof(long)
                || type == typeof(char)
                || type == typeof(uint));
        }

        public static bool IsCollectionType(this Type type)
        {
            return type.Name == "List`1";
        }
    }
}
