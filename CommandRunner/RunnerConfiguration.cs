using System;
using System.Collections.Generic;
using System.Reflection;
public class RunnerConfiguration
{
    internal string Title {get; private set;}
    internal ConsoleColor TerminalColor {get; private set;}
    internal ConsoleColor CommandColor {get; private set;}  
    internal RunModes? RunMode {get; set;}
    internal Func<Type, object> CommandActivator {get; private set;}
    internal List<Type> TypesToScan {get; private set;} = new List<Type>();
    public RunnerConfiguration (string title = "Command Runner")
    {
        Title = title;
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
        CommandActivator = (t) => Activator.CreateInstance(t);
    }

    public void ScanTypes(IEnumerable<Type> types) {
        TypesToScan.Clear();
        TypesToScan.AddRange(types);
    }

    public void ScanAssemblies(IEnumerable<Assembly> assemblies) {
        TypesToScan.Clear();
        var types = CommandRunner.Reflection.GetAllTypes(assemblies);
        TypesToScan.AddRange(types);
    }

    public void ScanAllAssemblies() {
        TypesToScan.Clear();
        var assemblies = CommandRunner.Reflection.GetAllReferencedAssemblies();
        var types = CommandRunner.Reflection.GetAllTypes(assemblies);
        TypesToScan.AddRange(types);
    }
}

internal enum RunModes {
    Terminal,
    CommandLine
}