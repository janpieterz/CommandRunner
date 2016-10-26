using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public class TypedParameterExecution
    {
        public static void Execute(object @class, MethodInfo methodInfo, List<string> arguments)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length > 0)
            {
                if (parameters.First().ParameterType == typeof(List<string>))
                {
                    methodInfo.Invoke(@class, new object[] { arguments });
                }
                else if (parameters.Length == arguments.Count)
                {
                    methodInfo.Invoke(@class, CreateTypedParameters(parameters, arguments));
                }
                else
                {
                    Console.WriteLine("We couldn't match the parameters.");
                }
            }
            else
            {
                methodInfo.Invoke(@class, null);
            }
        }
        private static object[] CreateTypedParameters(ParameterInfo[] parameters, List<string> arguments)
        {
            List<object> typedParameters = new List<object>();
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var argument = arguments[i];
                typedParameters.Add(CreateTypedParameter(parameter, argument));
            }
            return typedParameters.ToArray();
        }

        private static object CreateTypedParameter(ParameterInfo parameter, string argument)
        {
            if (IsPrimitiveType(parameter.ParameterType))
            {
                return ChangeType(parameter.ParameterType, argument);
            }
            else
            {
                return MakeType(parameter.ParameterType, argument);
            }
        }

        private static object MakeType(Type type, string value)
        {
            var ctor = type.GetTypeInfo().GetConstructor(new[] { typeof(string) });
            return ctor.Invoke(new object[] { value });
        }

        private static object ChangeType(Type type, string value)
        {
            if (type == typeof(bool))
            {
                return bool.Parse(value);
            }
            else if (type.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(type, value, true);
            }
            else
            {
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
        }

        private static bool IsPrimitiveType(Type type)
        {
            return (type.GetTypeInfo().IsValueType && type != typeof(Guid)) || type.GetTypeInfo().IsPrimitive ||
                   new[] { typeof(string), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan) }
                       .Contains(type) || Convert.GetTypeCode(type) != TypeCode.Object;
        }
    }
}