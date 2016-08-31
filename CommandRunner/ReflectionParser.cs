using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public class ReflectionParser
    {
        public class ReflectionSettings
        {
            public Func<Type, object> Activator { get; set; }
        }

        public ReflectionSettings Settings { get; set; } = new ReflectionSettings();

        public IEnumerable<IMenuItem> ReflectAllAssemblies()
        {
            List<ICommand> commands = new List<ICommand>();
            var assemblies = GetAssemblies();
            return ReflectAssemblies(assemblies);
        }

        public IEnumerable<IMenuItem> ReflectAssemblies(IEnumerable<Assembly> assemblies)
        {
            var types = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }
            return ReflectTypes(types);
        }

        private IEnumerable<IMenuItem> ReflectTypes(IEnumerable<Type> types)
        {
            var methods = new List<MethodInfo>();
            foreach (Type type in types)
            {
                methods.AddRange(type.GetMethods().Where(method => method.GetCustomAttribute<CommandAttribute>() != null));
            }
            return ReflectMethods(methods);
        }

        private IEnumerable<IMenuItem> ReflectMethods(IEnumerable<MethodInfo> methods)
        {
            var methodList = methods.ToList();
            var methodsWithAttributes = methodList.Where(method => method.GetCustomAttribute<CommandAttribute>() != null);
            var commandList = new List<IMenuItem>();
            foreach (MethodInfo methodInfo in methodsWithAttributes)
            {
                var commandAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();

                
                var attributeOnClass = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttribute<CommandAttribute>();
                var identifier = commandAttribute.Identifier;
                if (attributeOnClass != null)
                {
                    identifier = attributeOnClass.Identifier + " " + identifier;
                    commandList.Add(new ContainerCommand(attributeOnClass.Identifier, attributeOnClass.Help));
                }
                if (Settings.Activator != null)
                {
                    commandList.Add(new CustomActivatorCommand(identifier, commandAttribute.Help, methodInfo, Settings.Activator));
                }
                else
                {
                    commandList.Add(new ReflectedMethodCommand(identifier, commandAttribute.Help, methodInfo));
                }
            }
            return commandList;
        }

        private IEnumerable<Assembly> GetAssemblies()
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

        public ReflectionParser WithActivator(Func<Type, object> activator)
        {
            Settings.Activator = activator;
            return this;
        }
    }
}
