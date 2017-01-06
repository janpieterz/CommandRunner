using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner.ConsoleApplicationTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(Assembly.GetEntryAssembly().Location);
            RunnerConfiguration configuration = new RunnerConfiguration("Example Runner");
            configuration.ScanTypes(new List<Type>() { typeof(EchoCommand), typeof(NestingCommand) });
            configuration.ForceTerminal();
            Runner.Start(configuration);
        }
    }
}
