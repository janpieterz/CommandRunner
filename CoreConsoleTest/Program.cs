using System;
using System.Collections.Generic;
using System.Linq;
using CommandRunnerCoreTest;

namespace CommandRunner.CoreConsoleTest
{
    public class Program
    {
        private readonly List<object> items = new List<object>();
        public static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        private void Run()
        {
            new Runner(options =>
            {
                options.Title = "Command Runner";
                options.Scan.AllAssemblies();
                options.Activate.WithReflectionActivator();
                
            }).Run();
            //items.Add(new EchoCommand());
            //items.Add(new NestingCommand());
            //Runner.ScanAndStart("CommandRunners", Activator);
            ////Runner.Start("CommandRunnerCoreTest console", new List<ICommand>() {new EchoCommand()});
        }

        private object Activator(Type type)
        {
            return items.FirstOrDefault(item => item.GetType() == type);
        }
    }
}
