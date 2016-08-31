using Autofac;

namespace CommandRunner.CoreConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RunWithDefaults();
            RunWithAutofacCreator();
        }

        private static void RunWithDefaults()
        {
            new Runner().Run();
        }

        private static void RunWithAutofacCreator()
        {
            var container = CreateContainer();
            new Runner(options =>
            {
                options.Title = "Command Runner";
                options.Scan.AllAssemblies();
                options.Activate.WithCustomActivator(type => container.Resolve(type));
            }).Run();
        }

        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<EchoCommand>().PropertiesAutowired();
            builder.RegisterType<Injectable>().PropertiesAutowired();
            builder.RegisterType<NestingCommand>().PropertiesAutowired();
            return builder.Build();
        }
    }
}
