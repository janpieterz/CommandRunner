using System;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public static class ParameterInfoColoredWriterExtensions
    {
        public static void WriteToColoredConsole(this ParameterInfo parameterInfo, bool previousParameter)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(previousParameter ? ", " : " (");

            parameterInfo.ParameterType.SetConsoleColor();
            
            if (parameterInfo.ParameterType.GetTypeInfo().IsGenericType && parameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                //Process nullable Guid? etc
                var nulledType = parameterInfo.ParameterType.GetGenericArguments().FirstOrDefault();
                if (nulledType != null)
                {
                    if (nulledType.GetTypeInfo().IsValueType && nulledType != typeof(Guid))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                    }
                    nulledType.WriteTypeName();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("?");
                }
                else
                {
                    //Escape clause, should never be hit
                    parameterInfo.ParameterType.WriteTypeName();
                }
            }
            else if (parameterInfo.ParameterType.GetTypeInfo().IsGenericType)
            {
                //Process List<string> etc
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(parameterInfo.ParameterType.Name.Replace("`1", string.Empty));
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("<");
                var item = parameterInfo.ParameterType.GetGenericArguments().FirstOrDefault();
                item.SetConsoleColor();
                item.WriteTypeName();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(">");
            }
            else if (parameterInfo.ParameterType.IsArray)
            {
                parameterInfo.ParameterType.GetElementType().SetConsoleColor();
                parameterInfo.ParameterType.GetElementType().WriteTypeName();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[]");
            }
            else
            {
                //Process all other
                parameterInfo.ParameterType.WriteTypeName();
            }
            Console.Write(" ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(parameterInfo.Name);
            if (parameterInfo.IsOptional)
            {
                //Make sure to handle int x = 9 etc
                Console.Write(" = ");
                if (parameterInfo.DefaultValue == null)
                {
                    Console.Write("null");
                }
                else
                {
                    Console.Write(parameterInfo.DefaultValue);
                }
            }
        }

        public static void SetConsoleColor(this Type type)
        {
            if ((type.GetTypeInfo().IsValueType || type== typeof(string)) && (type != typeof(Guid)))
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