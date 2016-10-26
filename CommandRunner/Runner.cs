namespace CommandRunner
{
    public class Runner
    {
        public static void Start(RunnerConfiguration configuration) {
            var startableRunner = Create(configuration);
            startableRunner.Start();
        }
        public static IStartableRunner Create(RunnerConfiguration configuration) {
            InitializableRunner runner = new InitializableRunner(configuration);
            var startableRunner = runner.Initialize();
            return startableRunner;
        }
    }
}