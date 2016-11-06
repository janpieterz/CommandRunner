namespace CommandRunner.CommandLine
{
    public class CommandLineRunner : IStartableRunner {
        private CommandLineState _state;
        public CommandLineRunner (RunnerConfiguration configuration)
        {
            _state = new CommandLineState(configuration);
        }
        public void Start() {
            //     var commandWithArgs = InputParser.FindCommand(_settings.Menu.OfType<ICommand>(), arguments);
            //     commandWithArgs.Item1?.Execute(commandWithArgs.Item2.ToList());
            //     Console.WriteLine("Press enter to quit.");
            //     Console.ReadLine();
        }
    }
}