using System;
using System.Collections.Generic;

namespace CommandRunner.ClassicConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RunnerConfiguration configuration = new RunnerConfiguration("Example Runner");
            configuration.ScanTypes(new List<Type>() { typeof(EchoCommand), typeof(NestingCommand) });
            configuration.ForceTerminal();
            Runner.Start(configuration);
        }
    }
}
