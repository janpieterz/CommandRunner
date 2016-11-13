using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    internal static class TypeExtensions
    {
        internal static bool IsIList(this Type type)
        {
            return type.GetInterfaces()
                .Any(x => x == typeof(IList));
        }

        internal static Type GetEnumerableType(this Type type)
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

        internal static bool IsEnumerable(this Type type)
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
        internal static void SetConsoleColor(this Type type)
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

        internal static bool IsNullable(this Type type)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }
            return false;
        }

        internal static void WriteTypeName(this Type type)
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
            else if (type == typeof(decimal))
            {
                Console.Write("decimal");
            }
            else
            {
                Console.Write(type.Name);
            }
        }
    }
}