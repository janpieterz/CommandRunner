using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    internal class State
    {
        internal ConsoleColor StartupColor { get; set; }
        internal string Title { get; set; }
        internal ConsoleColor TerminalColor { get; set; }
        internal ConsoleColor CommandColor { get; set; }
        internal RunModes RunMode { get; set; }
        internal List<ICommand> ActiveMenu { get; set; }
        internal List<ICommand> FullMenu { get; set; }
        internal List<NavigatableCommand> NavigatableMenu => ActiveMenu.OfType<NavigatableCommand>().OrderBy(x => x.Identifier).ToList();
        internal List<SingleCommand> SingleCommands => ActiveMenu.OfType<SingleCommand>().OrderBy(x => x.Identifier).ToList();
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
            return instance;
        }
    }
    internal class ParentCommand
    {
        internal object Instance { get; set; }
        internal NavigatableCommand Command { get; set; }
    }
}