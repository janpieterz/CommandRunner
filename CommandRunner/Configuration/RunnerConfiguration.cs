using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandRunner.Configuration
{
    public interface IActivatableScanRunner
    {
        IScannableRunnerConfiguration WithReflectionActivator();
        IScannableRunnerConfiguration WithCustomActivator(Func<Type, object> activator);
    }

    public interface IScannableRunnerConfiguration
    {
        ICustomizableRunnerConfiguration ScanAllAssemblies();
        ICustomizableRunnerConfiguration ScanAssemblies(IEnumerable<Assembly> assemblies);
        ICustomizableRunnerConfiguration ScanTypes(IEnumerable<Type> types);
    }

    public interface ICustomizableRunnerConfiguration
    {
        IActivatableScanRunner Scan();
        ICustomizableRunnerConfiguration WithTitle(string title);
    }

    public class RunnerConfiguration : IActivatableScanRunner, IScannableRunnerConfiguration, ICustomizableRunnerConfiguration
    {
        public Func<Type, object> Activator { get; private set; }
        public bool ReflectionActivator { get; private set; }
        public bool AllAssemblies { get; private set; }
        public IEnumerable<Assembly> Assemblies { get; private set; }
        public IEnumerable<Type> Types { get; private set; }
        public string Title { get; private set; }

        public IActivatableScanRunner Scan()
        {
            return this;
        }

        public ICustomizableRunnerConfiguration WithTitle(string title)
        {
            Title = title;
            return this;
        }

        public ICustomizableRunnerConfiguration ScanAllAssemblies()
        {
            AllAssemblies = true;
            return this;
        }

        public ICustomizableRunnerConfiguration ScanAssemblies(IEnumerable<Assembly> assemblies)
        {
            Assemblies = assemblies;
            return this;
        }

        public ICustomizableRunnerConfiguration ScanTypes(IEnumerable<Type> types)
        {
            Types = types;
            return this;
        }
        public IScannableRunnerConfiguration WithReflectionActivator()
        {
            ReflectionActivator = true;
            return this;
        }

        public IScannableRunnerConfiguration WithCustomActivator(Func<Type, object> activator)
        {
            Activator = activator;
            return this;
        }

    }
}
