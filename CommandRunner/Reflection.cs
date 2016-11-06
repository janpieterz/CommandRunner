using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner
{
    public class Reflection
    {
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
