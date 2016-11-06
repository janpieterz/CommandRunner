using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner.Terminal {
    internal sealed class TerminalState : State {
        internal List<ICommand> Menu { get; private set; }
        internal ICommand ParentCommand { get; set; }
        internal Func<Type, object> CommandActivator { get; private set; }
        private Dictionary<Type, object> ActivatedTypes { get; set; } = new Dictionary<Type, object>();
        public TerminalState (RunnerConfiguration configuration) : base(configuration)
        {
            CommandActivator = StatefullCommandActivator;
            Menu = configuration.Menu;
        }

        private object StatefullCommandActivator(Type type)
        {
            if (ActivatedTypes.ContainsKey(type))
            {
                return ActivatedTypes[type];
            }
            var instance = Configuration.CommandActivator.Invoke(type);
            if(Configuration.Menu.Exists(c => c.Type == type && c is NavigatableCommand))
            {
                ActivatedTypes.Add(type, instance);
            }
            return instance;
        }

        public void SetMenu(List<ICommand> newMenu, ICommand parentCommand = null)
        {
            Menu = newMenu;
            ParentCommand = parentCommand;
        }
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