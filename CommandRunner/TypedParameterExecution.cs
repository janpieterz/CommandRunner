using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    internal class TypedParameterExecution
    {
        internal static object[] CreateTypedParameters(ParameterInfo[] parameters, List<string> arguments)
        {
            List<object> typedParameters = new List<object>();
            bool encounteredIList = parameters.Any() && parameters.LastOrDefault().ParameterType.IsIList();

            for (var i = 0; i < (encounteredIList ? parameters.Length - 1 : parameters.Length); i++)
            {
                var parameter = parameters[i];
                if (parameter == null) continue;
                if (parameter.ParameterType.IsIList())
                {
                    throw new Exception("Only the last parameter can be an IEnumerable.");
                }
                if (arguments.Count < i + 1)
                {
                    if (parameter.IsOptional)
                    {
                        typedParameters.Add(Type.Missing);
                    }
                    else if (parameter.ParameterType.IsNullable())
                    {
                        typedParameters.Add(null);
                    }
                    continue;
                }
                
                var argument = arguments[i];
                typedParameters.Add(CreateTypedParameter(parameter.ParameterType, argument));
            }

            if (encounteredIList)
            {
                var parameter = parameters[parameters.Length - 1];
                
                var argumentsLeft = arguments.Skip(parameters.Length - 1).ToList();
                var enumerableType = parameter.ParameterType.GetEnumerableType();
                
                
                List<object> typedArguments = new List<object>();
                foreach (string argument in argumentsLeft)
                {
                    var typedArgument = CreateTypedParameter(enumerableType, argument);
                    typedArguments.Add(typedArgument);
                }
                
                var typedEnumerable = (IList)MakeIEnumerable(parameter.ParameterType, typedArguments);

                typedParameters.Add(typedEnumerable);
            }
            return typedParameters.ToArray();
        }

        private static object CreateTypedParameter(Type type, string argument)
        {
            return IsPrimitiveType(type) ? ChangeType(type, argument) : MakeType(type, argument);
        }

        private static object MakeType(Type type, string value)
        {
            var ctor = type.GetTypeInfo().GetConstructor(new[] { typeof(string) });
            return ctor.Invoke(new object[] { value });
        }

        private static object MakeIEnumerable(Type type, List<object> items)
        {
            if (type.IsArray)
            {
                var array = Array.CreateInstance(type.GetElementType(), items.Count);
                for (var i = 0; i < items.Count; i++)
                {
                    array.SetValue(items[i], i);
                }
                return array;
            }
            var ctor = type.GetTypeInfo().GetConstructor(new Type[0]);
            var enumerable = (IList) ctor.Invoke(new object[] {  });
            foreach (var item in items)
            {
                enumerable.Add(item);
            }
            return enumerable;
        }

        private static object ChangeType(Type type, string value)
        {
            if (type == typeof(bool))
            {
                return bool.Parse(value);
            }
            if (type.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(type, value, true);
            }
            if (type.IsNullable())
            {
                var nulledType = type.GetGenericArguments().FirstOrDefault();
                if (nulledType == null)
                {
                    throw new Exception($"Cannot process nullably type without a generic argument. Type: {type.Name}");
                }
                var result = CreateTypedParameter(nulledType, value);
                return result;
            }
            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        private static bool IsPrimitiveType(Type type)
        {
            return (type.GetTypeInfo().IsValueType && type != typeof(Guid)) || type.GetTypeInfo().IsPrimitive ||
                   new[] { typeof(string), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan) }
                       .Contains(type) || Convert.GetTypeCode(type) != TypeCode.Object;
        }
    }
}