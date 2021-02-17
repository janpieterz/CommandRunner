using System;
using System.Collections.Generic;

namespace CommandRunner.Net5Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var envArgs = Environment.GetCommandLineArgs();
            RunnerConfiguration configuration = new RunnerConfiguration("Example Runner");
            configuration.ScanTypes(new List<Type>() { typeof(EchoCommand), typeof(NestingCommand), typeof(RandomMenu) });
            configuration.AddTypes(new List<Type>(){ typeof(ExamplePublic)}, true);
            Runner.Start(configuration);
        }
    }
}
