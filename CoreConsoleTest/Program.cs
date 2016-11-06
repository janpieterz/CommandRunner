using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using CommandRunner;
namespace CommandRunner.CoreConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
                    {
            RunnerConfiguration configuration = new RunnerConfiguration("Testing config");
            configuration.ScanAllAssemblies();
            configuration.UseReflectionCommandActivator();
            configuration.ForceTerminal();
            Runner.Start(configuration);
            // RunWithDefaults();
            //RunWithAutofacCreator();
            //RunWithProvidedAssemblies();
            //RunWithProvidedTypes();
            //RunWithProvidedMenu();
            //RunWithAllOptions();
        }
        //private static void RunWithDefaults()
        //{
        //    new Runner().Run();
        //}

        //private static void RunWithAutofacCreator()
        //{
        //    var container = CreateContainer();
        //    new Runner(options =>
        //    {
        //        options.Title = "Command Runner";
        //        options.Scan.AllAssemblies();
        //        options.Activate.WithCustomActivator(type => container.Resolve(type));
        //    }).Run();
        //}
        //private static void RunWithAllOptions()
        //{
        //    //This does not work but is purely to display all options.
        //    new Runner(options =>
        //    {
        //        options.Title = "Command Runner";
        //        options.Scan.AllAssemblies();
        //        options.Scan.SpecificAssemblies(new List<Assembly>());
        //        options.Scan.SpecificTypes(new List<Type>());
        //        options.Activate.WithCustomActivator(type => null);
        //        options.Activate.WithReflectionActivator();
        //        options.ForceCommandLine = true;
        //        options.ForceTerminal = true;
        //        options.UseMenuItems(new List<IMenuItem>());
        //        options.CommandColor = ConsoleColor.White;
        //        options.RunnerColor = ConsoleColor.Green;
        //    }).Run();
        //}

        //private static void RunWithProvidedMenu()
        //{
        //    new Runner(options =>
        //    {
        //        options.UseMenuItems(new List<IMenuItem>()
        //        {
        //            new ReflectedMethodCommand("reflect", "Custom reflected method command", GetTestMethodInfo()),
        //            new CustomActivatorCommand("activator", "Custom activator method command", GetTestMethodInfo(), type => new NestingCommand())
        //        }); 
        //    }).Run();
        //}

        //private static void RunWithProvidedTypes()
        //{
        //    new Runner(options =>
        //    {
        //        options.Scan.SpecificTypes(new List<Type>() { typeof(EchoCommand)});
        //    }).Run();
        //}

        //private static void RunWithProvidedAssemblies()
        //{
        //    new Runner(options =>
        //    {
        //        options.Scan.SpecificAssemblies(new List<Assembly>() { Assembly.GetEntryAssembly() });
        //    }).Run();
        //}
        //private static IContainer CreateContainer()
        //{
        //    var builder = new ContainerBuilder();
        //    builder.RegisterType<EchoCommand>().PropertiesAutowired();
        //    builder.RegisterType<Injectable>().PropertiesAutowired();
        //    builder.RegisterType<NestingCommand>().PropertiesAutowired();
        //    return builder.Build();
        //}

        //private static MethodInfo GetTestMethodInfo()
        //{
        //    return typeof(NestingCommand).GetMethod(nameof(NestingCommand.Hello));
        //}
    }
}
