using System;
using System.Reflection;

namespace CommandRunner.ConsoleApplicationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Assembly.GetEntryAssembly().Location);
            var arguments = Environment.GetCommandLineArgs();
            new Runner(options =>
            {
                options.Title = "Command Runner";
                options.Scan.AllAssemblies();
                options.Activate.WithReflectionActivator();

            }).Run();
        }
    }
}
