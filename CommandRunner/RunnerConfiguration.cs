using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public class RunnerConfiguration
    {
        internal string Title {get; private set;}
        internal ConsoleColor TerminalColor {get; private set;} = ConsoleColor.Green;
        internal ConsoleColor CommandColor { get; private set; } = ConsoleColor.White;
        internal RunModes? RunMode {get; set;}
        internal Func<Type, object> CommandActivator {get; private set;}
        internal List<Type> TypesToScan {get;} = new List<Type>();
        internal List<string> Arguments { get; set; }

        public RunnerConfiguration (string title = "Command Runner")
        {
            Title = title;
            UseReflectionCommandActivator();
            ScanAllAssemblies();
        }

        public void UseTerminalColor(ConsoleColor color) {
            TerminalColor = color;
        }

        public void UseCommandColor(ConsoleColor color) {
            CommandColor = color;
        }
    
        public void ForceTerminal() {
            RunMode = RunModes.Terminal;
        }

        public void ForceCommandLine() {
            RunMode = RunModes.CommandLine;
        }

        public void UseCommandActivator(Func<Type, object> commandActivator) {
            CommandActivator = commandActivator;
        }

        public void UseReflectionCommandActivator() {
            CommandActivator = Activator.CreateInstance;
        }

        public void UseArguments(IEnumerable<string> arguments)
        {
            Arguments = arguments.ToList();
        }
        public void ScanTypes(IEnumerable<Type> types) {
            TypesToScan.Clear();
            TypesToScan.AddRange(types);
        }

        public void ScanAssemblies(IEnumerable<Assembly> assemblies) {
            TypesToScan.Clear();
            var types = Reflection.GetAllTypes(assemblies);
            TypesToScan.AddRange(types);
        }

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