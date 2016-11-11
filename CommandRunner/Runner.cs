
using System;

namespace CommandRunner
{
    public class Runner
    {
        public static void Start(RunnerConfiguration configuration) {
            try
            {
                var startableRunner = Create(configuration);
                startableRunner?.Start();
            }
            catch (Exception exception)
            {
                ConsoleWrite.WriteErrorLine(exception.Message);
            }
        }
        public static IStartableRunner Create(RunnerConfiguration configuration) {
            InitializableRunner runner = new InitializableRunner(configuration);
            var startableRunner = runner.Initialize();
            return startableRunner;
        }
    }
}