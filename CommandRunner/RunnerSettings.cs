using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public class RunnerSettings : IConfigureActivation, IConfigureScanning, ICustomizableRunnerConfiguration
    {
        public Func<Type, object> Activator { get; internal set; }
        public bool ReflectionActivator { get; internal set; }
        public bool ScanAllAssemblies { get; internal set; }
        public IReadOnlyCollection<Assembly> SpecificAssembliesToScan { get; internal set; }
        public IReadOnlyCollection<Type> SpecificTypesToScan { get; internal set; }
        public string Title { get; set; }
        public bool ForceTerminal { get; set; }
        public bool ForceCommandLine { get; set; }
        public void UseMenuItems(IEnumerable<IMenuItem> menuItems)
        {
            Menu = menuItems.ToList();
        }

        internal List<IMenuItem> Menu { get; set; } = new List<IMenuItem>();
        internal RunMode Mode { get; set; }
        public IConfigureActivation Activate => this;

        IConfigureScanning ICustomizableRunnerConfiguration.Scan => this;

        public void AllAssemblies()
        {
            ScanAllAssemblies = true;
        }

        public void SpecificAssemblies(IEnumerable<Assembly> assemblies)
        {
            SpecificAssembliesToScan = new ReadOnlyCollection<Assembly>(assemblies.ToList());
        }

        public void SpecificTypes(IEnumerable<Type> types)
        {
            SpecificTypesToScan = new ReadOnlyCollection<Type>(types.ToList());
        }

        ICustomizableRunnerConfiguration IConfigureActivation.WithCustomActivator(Func<Type, object> activator)
        {
            if (ReflectionActivator)
            {
                throw new Exception("Cannot use custom activator with a reflection activator. Please make sure to only set one activator.");
            }
            Activator = activator;
            return this;
        }

        ICustomizableRunnerConfiguration IConfigureActivation.WithReflectionActivator()
        {
            if (Activator != null)
            {
                throw new Exception("Cannot use reflection activator with a custom activator. Please make sure to only set one  activator.");
            }
            ReflectionActivator = true;
            return this;
        }
    }

    public interface IConfigureActivation
    {
        ICustomizableRunnerConfiguration WithReflectionActivator();
        ICustomizableRunnerConfiguration WithCustomActivator(Func<Type, object> activator);
        bool ReflectionActivator { get; }
        Func<Type, object> Activator { get; }
    }

    public interface IConfigureScanning
    {
        void AllAssemblies();
        void SpecificAssemblies(IEnumerable<Assembly> assemblies);
        void SpecificTypes(IEnumerable<Type> types);
        bool ScanAllAssemblies { get;}
        IReadOnlyCollection<Assembly> SpecificAssembliesToScan { get; }
        IReadOnlyCollection<Type> SpecificTypesToScan { get; }

    }

    public interface ICustomizableRunnerConfiguration
    {
        IConfigureActivation Activate { get; }
        IConfigureScanning Scan { get; }
        string Title { get; set; }
        bool ForceTerminal { get; set; }
        bool ForceCommandLine { get; set; }
        void UseMenuItems(IEnumerable<IMenuItem> menuItems);
    }

    internal enum RunMode
    {
        Terminal,
        CommandLine
    }
}
