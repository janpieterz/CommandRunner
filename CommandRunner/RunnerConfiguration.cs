using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    /// <summary>
    /// Configuration used to create a runner
    /// </summary>
    public class RunnerConfiguration
    {
        internal string Title {get; private set;}
        internal ConsoleColor TerminalColor {get; private set;} = ConsoleColor.Green;
        internal ConsoleColor CommandColor { get; private set; } = ConsoleColor.White;
        internal RunModes? RunMode {get; set;}
        internal Func<Type, object> CommandActivator {get; private set;}
        internal List<Type> TypesToScan {get;} = new List<Type>();
        internal List<string> Arguments { get; set; }
        /// <summary>
        /// Initializes the RunnerConfiguration builder.
        /// </summary>
        /// <param name="title"></param>
        public RunnerConfiguration (string title = "Command Runner")
        {
            Title = title;
            CommandActivator = Activator.CreateInstance;
            ScanAllAssemblies();
        }
        /// <summary>
        /// Set the color used to show the Terminal in. Defaults to bright green.
        /// </summary>
        public void UseTerminalColor(ConsoleColor color) {
            TerminalColor = color;
        }
        /// <summary>
        /// Set the color used when running the commands. Defaults to white;
        /// </summary>
        /// <param name="color"></param>
        public void UseCommandColor(ConsoleColor color) {
            CommandColor = color;
        }
        /// <summary>
        /// Force running in terminal mode.
        /// </summary>
        public void ForceTerminal() {
            RunMode = RunModes.Terminal;
        }
        /// <summary>
        /// Force running in command line mode.
        /// </summary>
        public void ForceCommandLine() {
            RunMode = RunModes.CommandLine;
        }
        /// <summary>
        /// Provide your own activator instead of the reflection command activator.
        /// </summary>
        public void UseCommandActivator(Func<Type, object> commandActivator) {
            CommandActivator = commandActivator;
        }
        /// <summary>
        /// Provide your own arguments instead of parsing environment arguments
        /// </summary>
        public void UseArguments(IEnumerable<string> arguments)
        {
            Arguments = arguments.ToList();
        }
        /// <summary>
        /// Provide the types to scan
        /// </summary>
        public void ScanTypes(IEnumerable<Type> types) {
            TypesToScan.Clear();
            TypesToScan.AddRange(types);
        }
        /// <summary>
        /// Provide assemblies to scan for types
        /// </summary>
        public void ScanAssemblies(IEnumerable<Assembly> assemblies) {
            TypesToScan.Clear();
            var types = Reflection.GetAllTypes(assemblies);
            TypesToScan.AddRange(types);
        }
        /// <summary>
        /// Scan all assemblies (default)
        /// </summary>
        public void ScanAllAssemblies() {
            TypesToScan.Clear();
            var assemblies = Reflection.GetAllReferencedAssemblies();
            var types = Reflection.GetAllTypes(assemblies);
            TypesToScan.AddRange(types);
        }
    }

    internal enum RunModes {
        Terminal,
        CommandLine
    }
}