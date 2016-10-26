using System.Collections.Generic;

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
        }
        //List of scanned results
        //retrieval methods for specific items
    }
}