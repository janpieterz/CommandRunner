using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public class Reflection
    {

        public static IEnumerable<MethodInfo> ReflectAllAssemblies()
        {
            var assemblies = GetAssemblies();
            return ReflectAssemblies(assemblies);
        }

        public static IEnumerable<MethodInfo> ReflectAssemblies(IEnumerable<Assembly> assemblies)
        {
            var types = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }
            return ReflectTypes(types);
        }

        public static IEnumerable<MethodInfo> ReflectTypes(IEnumerable<Type> types)
        {
            var methods = new List<MethodInfo>();
            foreach (Type type in types)
            {
                methods.AddRange(type.GetMethods().Where(method => method.GetCustomAttribute<CommandAttribute>() != null));
            }
            return ReflectMethods(methods);
        }

        private static IEnumerable<MethodInfo> ReflectMethods(IEnumerable<MethodInfo> methods)
        {
            var methodList = methods.ToList();
            var methodsWithAttributes = methodList.Where(method => method.GetCustomAttribute<CommandAttribute>() != null);
            return methodsWithAttributes;
        }

        private static IEnumerable<Assembly> GetAssemblies()
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
    }
}
