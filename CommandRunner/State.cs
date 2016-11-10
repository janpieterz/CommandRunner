using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandRunner.Terminal;

namespace CommandRunner
{
    internal class State
    {
        internal ConsoleColor StartupColor { get; set; }
        internal string Title { get; set; }
        internal ConsoleColor TerminalColor { get; set; }
        internal ConsoleColor CommandColor { get; set; }
        internal RunModes RunMode { get; set; }
        internal List<ICommand> Menu { get; set; }
        internal List<NavigatableCommand> NavigatableMenu => Menu.OfType<NavigatableCommand>().ToList();
        internal List<SingleCommand> SingleCommands => Menu.OfType<SingleCommand>().ToList();
        internal List<Type> NavigatableTypes { get; set; }
        internal Func<Type, object> CommandActivator { private get; set; }
        internal List<string> Arguments { get; set; }
        internal Dictionary<Type, ParentCommand> ParentHierarchy { get; } = new Dictionary<Type, ParentCommand>();

        internal object StatefullCommandActivator(Type type)
        {
            if (ParentHierarchy.ContainsKey(type))
            {
                return ParentHierarchy[type].Instance;
            }

            var instance = CommandActivator.Invoke(type);

            foreach (var property in type.GetProperties())
            {
                if (ParentHierarchy.ContainsKey(property.PropertyType))
                {
                    property.SetValue(instance, ParentHierarchy[property.PropertyType].Instance);
                }
            }

            if (NavigatableTypes.Exists(c => c == type))
            {
                ParentHierarchy.Add(type, new ParentCommand
                {
                    Instance = instance
                });
            }
            return instance;
        }
    }
    public class ParentCommand
    {
        public object Instance { get; set; }
        public NavigatableCommand Command { get; set; }
    }
}