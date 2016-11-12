
using System;

namespace CommandRunner
{
    /// <summary>
    /// Helper to create runners.
    /// </summary>
    public class Runner
    {
        /// <summary>
        /// Start a runner right away
        /// </summary>
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
        /// <summary>
        /// Create a runner without starting it
        /// </summary>
        public static IStartableRunner Create(RunnerConfiguration configuration) {
            InitializableRunner runner = new InitializableRunner(configuration);
            var startableRunner = runner.Initialize();
            return startableRunner;
        }
    }
}