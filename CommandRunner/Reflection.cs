using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public class Reflection
    {
        public static IEnumerable<MethodInfo> FindCommands(IEnumerable<Type> types)
        {
            var methods = new List<MethodInfo>();
            foreach (Type type in types)
            {
                var customAttribute = type.GetTypeInfo().CustomAttributes.ToList()
                    .Where(x => x.AttributeType == typeof(NavigatableCommandAttribute) 
                        || x.AttributeType == typeof(NestedCommandAttribute)).FirstOrDefault();
                        
                if(customAttribute != null) {

                }
                //Type can be a nested or navigatable Command
                //Method can be a command attribute
                methods.AddRange(type.GetMethods().Where(method => method.GetCustomAttribute<CommandAttribute>() != null));
            }
            return methods;
        }

        public static IEnumerable<Assembly> GetAllReferencedAssemblies()
        {
            var assemblyNames = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            var assemblies = new List<Assembly>();
            foreach (var name in assemblyNames)
            {
                assemblies.Add(Assembly.Load(name));
            }
            assemblies.Add(Assembly.GetEntryAssembly());
            return assemblies;
        }

        public static IEnumerable<Type> GetAllTypes(IEnumerable<Assembly> assemblies) {
            var types = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }
            return types;
        }
    }
}
