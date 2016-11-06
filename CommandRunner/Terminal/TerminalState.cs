using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner.Terminal {
    internal sealed class TerminalState : State {
        public TerminalState (RunnerConfiguration configuration) : base(configuration)
        {
          
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