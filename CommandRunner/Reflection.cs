using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner
{
    internal class Reflection
    {
        internal static IEnumerable<Assembly> GetAllReferencedAssemblies()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                var assemblyNames = entryAssembly.GetReferencedAssemblies();
                var assemblies = new List<Assembly>();
                foreach (var name in assemblyNames)
                {
                    assemblies.Add(Assembly.Load(name));
                }
                assemblies.Add(Assembly.GetEntryAssembly());
                return assemblies;
            }
            return new Assembly[0];
        }

        internal static IEnumerable<Type> GetAllTypes(IEnumerable<Assembly> assemblies) {
            var types = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }
            return types;
        }
    }
}
