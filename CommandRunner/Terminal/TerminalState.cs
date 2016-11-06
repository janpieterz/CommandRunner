using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner.Terminal {
    internal sealed class TerminalState : State {
        internal List<ICommand> Menu { get; private set; }
        internal List<NavigatableCommand> NavigatableMenu => Menu.OfType<NavigatableCommand>().ToList();
        internal List<SingleCommand> SingleCommands => Menu.OfType<SingleCommand>().ToList();
        internal Func<Type, object> CommandActivator { get; private set; }
        internal Dictionary<Type, ParentCommand> ParentHierarchy { get; } = new Dictionary<Type, ParentCommand>();
        public TerminalState (RunnerConfiguration configuration) : base(configuration)
        {
            CommandActivator = StatefullCommandActivator;
            Menu = configuration.Menu;
        }

        private object StatefullCommandActivator(Type type)
        {
            if (ParentHierarchy.ContainsKey(type))
            {
                return ParentHierarchy[type].Instance;
            }
            var instance = Configuration.CommandActivator.Invoke(type);

            foreach (var property in type.GetProperties())
            {
                if (ParentHierarchy.ContainsKey(property.PropertyType))
                {
                    property.SetValue(instance, ParentHierarchy[property.PropertyType].Instance);
                }
            }

            if(Configuration.NavigatableTypes.Exists(c => c == type))
            {
                ParentHierarchy.Add(type, new ParentCommand
                {
                    Instance = instance
                });
            }
            return instance;
        }

        public void SetMenu(List<ICommand> newMenu, ICommand parentCommand = null)
        {
            Menu = newMenu;
            var command = parentCommand as NavigatableCommand;
            if (command != null && ParentHierarchy.ContainsKey(parentCommand.Type))
            {
                ParentHierarchy[parentCommand.Type].Command = command;
            }
        }

        public void MoveUp()
        {
            if (!ParentHierarchy.Any()) return;
            var previous = ParentHierarchy.Last();
            ParentHierarchy.Remove(previous.Key);
            if (ParentHierarchy.Any())
            {
                Menu = ParentHierarchy.Last().Value.Command.SubItems;
            }
            else
            {
                Menu = Configuration.Menu;

            }
        }
    }

    public class ParentCommand
    {
        public object Instance { get; set; }
        public NavigatableCommand Command { get; set; }
    }
}

namespace CommandRunner {
    internal abstract class State {
        internal RunnerConfiguration Configuration {get; private set;}
        internal List<string> Arguments {get; set;}
        public State (RunnerConfiguration configuration)
        {
            Configuration = configuration;
            Arguments = configuration.Arguments ?? Environment.GetCommandLineArgs().ToList();
        }
        //List of scanned results
        //retrieval methods for specific items
    }
}