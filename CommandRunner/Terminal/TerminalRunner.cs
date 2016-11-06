using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner.Terminal
{
    public class TerminalRunner : IStartableRunner {
        private readonly RunnerConfiguration _configuration;
        private readonly TerminalState _state;
        public TerminalRunner (RunnerConfiguration configuration)
        {
            _configuration = configuration;
            _state = new TerminalState(_configuration);
            Console.Title = configuration.Title;
        }
        public void Start()
        {
            string input;
            do
            {
                Console.ForegroundColor = _configuration.TerminalColor;
                PrintLine();
                PrintMenu();
                input = QueryForcommand();

                var arguments = InputParser.ParseInputToArguments(input).ToList();
                Console.WriteLine();
                if (!arguments.Any())
                {
                    ConsoleWrite.WriteErrorLine("Please provide an argument");
                    continue;
                }
                if (arguments[0] == "help")
                {
                    var identifier = input.Split(' ')[1];
                    var item = _state.NavigatableMenu.FirstOrDefault(x => x.Identifier == identifier);
                    if (item != null)
                    {
                        Console.WriteLine("Help for: {0}", item.Identifier);
                        PrintNavigatableItems(item.SubItems.OfType<NavigatableCommand>().ToList());
                        PrintSingleCommands(item.SubItems.OfType<SingleCommand>().ToList());
                        
                        Console.WriteLine();
                        Console.Write("Press enter to return to the menu");
                        Console.ReadLine();
                    }
                    else
                    {
                        ConsoleWrite.WriteErrorLine("Make sure you spelled the menu item correctly.");
                    }
                    continue;
                }
                if (arguments[0] == "up")
                {
                    _state.MoveUp();
                    continue;
                }

                var matches =
                    _state.Menu.Select(x => new {Key = x, Value = x.Match(arguments)})
                        .Where(x => x.Value != MatchState.Miss)
                        .ToDictionary(pair => pair.Key, pair => pair.Value);
                if (!matches.Any())
                {
                    ConsoleWrite.WriteErrorLine("Please provide a valid command.");
                    continue;
                }
                foreach (KeyValuePair<ICommand, MatchState> match in matches)
                {
                    if (match.Value == MatchState.MissingParameter)
                    {
                        ConsoleWrite.WriteErrorLine("Make sure you provide all the arguments for your command:");
                        match.Key.WriteToConsole();
                    }
                    else if (match.Value == MatchState.TooManyParameters)
                    {
                        ConsoleWrite.WriteErrorLine("Looks like you provided too much parameters for your command:");
                        match.Key.WriteToConsole();
                    }
                    else if (match.Value == MatchState.WrongTypes)
                    {
                        ConsoleWrite.WriteErrorLine("The provided types did not match the method parameters!");
                    }
                    else if (match.Value == MatchState.Matched)
                    {
                        ExecuteCommand(match.Key, arguments);
                    }
                }

            } while (string.IsNullOrEmpty(input) || !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase));

            
            Console.ReadLine();
        }

        private string QueryForcommand()
        {
            Console.Write($"{Environment.NewLine}Command> ");
            Console.ForegroundColor = _configuration.CommandColor;
            return Console.ReadLine() ?? string.Empty;
        }

        private void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            string menuIdentifier = "Main";
            if (_state.ParentHierarchy.Any())
            {
                menuIdentifier = _state.ParentHierarchy.Last().Value.Command.Identifier;
            }
            Console.WriteLine($"{menuIdentifier} menu:");
            Console.ForegroundColor = _configuration.TerminalColor;

            if (_state.NavigatableMenu.Any())
            {
                PrintNavigatableItems(_state.NavigatableMenu);
            }

            if (_state.SingleCommands.Any())
            {
                PrintSingleCommands(_state.SingleCommands);
            }

            if (_state.ParentHierarchy.Any())
            {
                Console.WriteLine("To go to the previous menu type `up`");
            }
        }

        private void PrintNavigatableItems(List<NavigatableCommand> commands)
        {
            Console.WriteLine("Sub-menu's available (type help x to print sub items):");
            foreach (ICommand command in commands.OrderBy(x => x.Identifier))
            {
                Console.Write("  ");
                command.WriteToConsole();
            }
            Console.WriteLine();
        }

        private void PrintSingleCommands(List<SingleCommand> commands)
        {
            Console.WriteLine("Commands: ");
            foreach (SingleCommand command in commands)
            {
                PrintCommand(command);
            }
        }

        private void PrintCommand(SingleCommand command)
        {
            Console.Write("  ");
            command.WriteToConsole();
        }

        private void PrintLine()
        {
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("-");
            }
        }

        private void ExecuteCommand(ICommand command, List<string> arguments )
        {
            try
            {
                var commandInstance = _state.CommandActivator.Invoke(command.Type);
                Console.ForegroundColor = _configuration.CommandColor;
                if (command.Parameters.Count > 0)
                {
                    var typedParameters =
                        TypedParameterExecution.CreateTypedParameters(command.Parameters.ToArray(),
                            command.ArgumentsWithoutIdentifier(arguments));
                    
                    command.MethodInfo.Invoke(commandInstance, typedParameters);

                }
                else
                {
                    command.MethodInfo.Invoke(commandInstance, null);
                }
                var navigatableCommand = command as NavigatableCommand;
                if (navigatableCommand != null)
                {
                    _state.SetMenu(navigatableCommand.SubItems, navigatableCommand);
                }
            }
            catch (Exception exception)
            {
                ConsoleWrite.WriteErrorLine($"We couldn't setup your command parameters. Exception: {exception.Message}");
            }
            finally
            {
                Console.ForegroundColor = _configuration.TerminalColor;
            }
        }
    }
}