using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner
{
    /// <summary>
    /// Runner to execute the commands.
    /// </summary>
    public class Runner
    {
        private ICommand[] _commands;
        /// <summary>
        /// Create your command runner.
        /// </summary>
        /// <param name="title">Optional title for the console.</param>
        public Runner(string title = "Command Runner")
        {
#if DOTNET5_4
            //As soon as support for this lands in .net core
            //https://github.com/dotnet/corefx/issues/4636
#else
            Console.Title = title;
#endif
        }
        /// <summary>
        /// Run the command runner
        /// </summary>
        /// <param name="commands">All commands you'd like to be made available.</param>
        public void Run(params ICommand[] commands)
        {
            if (!commands.Any())
            {
                Console.WriteLine("Please add some commands to add functionality.");
                return;
            }
            _commands = commands;
            string input;
            do
            {
                Console.WriteLine("Available commands:");
                List<IEnumerable<string>> helpItems = new List<IEnumerable<string>>();
                foreach (var helpItem in _commands.SelectMany(command => command.Help))
                {
                    ConsoleMessages.Help(helpItem);
                }
                Console.WriteLine("command: ");
                input = Console.ReadLine() ?? string.Empty;
                Process(input);
            } while (string.IsNullOrEmpty(input) || !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase));
        }

        private void Process(string input)
        {
            string[] arguments = ParseArguments(input);

            if (arguments == null) return;

            string command = arguments[0].ToLowerInvariant();

            ICommand handler = _commands.FirstOrDefault(x => x.Command.ToLower() == command);
            if (handler == null)
            {
                ConsoleMessages.Error($"Command {command} not recognized!");
            }
            else
            {
                var commandArgs = arguments.Skip(1);
                handler.Execute(commandArgs);
            }
        }

        private string[] ParseArguments(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            //Parse arguments with a double quote in it as a single item, for example: File copy "C:\Program Files\example.txt"
            var arguments = input.Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                    : new string[] { element })  // Keep the entire item
                .SelectMany(element => element).ToArray();
            if (!arguments.Any()) return null;
            return arguments;
        }
    }
}