using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public static class TypeExtensions
    {
        public static bool IsIList(this Type type)
        {
            return type.GetInterfaces()
                .Any(x => x == typeof(IList));
        }

        public static Type GetEnumerableType(this Type type)
        {
            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.GetTypeInfo().IsGenericType
                    && @interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return @interface.GetGenericArguments()[0];
                }
            }
            return null;
        }

        public static bool IsEnumerable(this Type type)
        {
            if (type == typeof(string)) return false;
            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.GetTypeInfo().IsGenericType
                    && @interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return true;
                }
            }
            return false;
        }
        public static void SetConsoleColor(this Type type)
        {
            if ((type.GetTypeInfo().IsValueType || type == typeof(string)) && (type != typeof(Guid)))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
        }

        public static void WriteTypeName(this Type type)
        {
            if (type == typeof(string))
            {
                Console.Write("string");
            }
            else if (type == typeof(int))
            {
                Console.Write("int");
            }
            else if (type == typeof(bool))
            {
                Console.Write("bool");
            }
            else
            {
                Console.Write(type.Name);
            }
        }
    }
}