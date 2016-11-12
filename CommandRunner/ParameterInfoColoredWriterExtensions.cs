using System;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    internal static class ParameterInfoColoredWriterExtensions
    {
        internal static void WriteToColoredConsole(this ParameterInfo parameterInfo, bool previousParameter)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(previousParameter ? ", " : " (");

            parameterInfo.ParameterType.SetConsoleColor();
            
            if (parameterInfo.ParameterType.IsNullable())
            {
                //Process nullable Guid? etc
                var nulledType = parameterInfo.ParameterType.GetGenericArguments().Single();
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
    }
}