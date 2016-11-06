using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner.Terminal
{
    public class TerminalRunner : IStartableRunner {
        private readonly RunnerConfiguration _configuration;

        public TerminalRunner (RunnerConfiguration configuration)
        {
            _configuration = configuration;
            Console.Title = configuration.Title;
        }
        public void Start()
        {
            string input;
            Console.ForegroundColor = _configuration.TerminalColor;
            do
            {
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write("-");
                }

                var menuItems = _configuration.Menu.OfType<NavigatableCommand>().OrderBy(x => x.Identifier);
                if (menuItems.Any())
                {
                    Console.WriteLine("Menu's available (type help x to print sub items):");
                    foreach (IWritableMenuItem command in _configuration.Menu.OfType<NavigatableCommand>().OrderBy(x => x.Identifier))
                    {
                        Console.Write("  ");
                        command.WriteToConsole();
                    }
                    Console.WriteLine();
                }

                var commands = _configuration.Menu.OfType<Command>().OrderBy(x => x.Identifier);

                if (commands.Any())
                {
                    Console.WriteLine("Commands: ");
                    foreach (IWritableMenuItem command in commands)
                    {
                        Console.Write("  ");
                        command.WriteToConsole();
                    }
                }
                
                Console.Write($"{Environment.NewLine}Command> ");
                input = Console.ReadLine() ?? string.Empty;

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
                    var item = menuItems.FirstOrDefault(x => x.Identifier == identifier);
                    if (item != null)
                    {
                        Console.WriteLine("MENU:");
                        item.WriteToConsole();
                        Console.WriteLine();
                        item.SubItems.ForEach(x => x.WriteToConsole());

                        Console.WriteLine();
                        Console.Write("Press enter to return to the menu");
                        Console.ReadLine();
                        continue;
                    }
                    else
                    {
                        ConsoleWrite.WriteErrorLine("Make sure you spelled the menu item correctly.");
                    }
                    continue;
                }

                var matches =
                    _configuration.Menu.Select(x => new {Key = x, Value = x.Match(arguments)})
                        .Where(x => x.Value != MatchState.Miss)
                        .ToDictionary(pair => pair.Key, pair => pair.Value);
                if (!matches.Any())
                {
                    ConsoleWrite.WriteErrorLine("Please provide a valid command.");
                    continue;
                }
                foreach (KeyValuePair<IWritableMenuItem, MatchState> match in matches)
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
                    else if (match.Value == MatchState.Matched)
                    {
                        ExecuteCommand(match.Key, arguments);

                        Console.WriteLine("MATCHED!");
                    }
                    else if (match.Value == MatchState.WrongTypes)
                    {
                        ConsoleWrite.WriteErrorLine("The provided types did not match the method parameters!");
                    }
                }

            } while (string.IsNullOrEmpty(input) || !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase));

            
            Console.ReadLine();
        }

        private void ExecuteCommand(IWritableMenuItem menuItem, List<string> arguments )
        {
            try
            {
                var typedParameters =
                    TypedParameterExecution.CreateTypedParameters(menuItem.Parameters.ToArray(),
                        menuItem.ArgumentsWithoutIdentifier(arguments));


            }
            catch (Exception exception)
            {
                ConsoleWrite.WriteErrorLine($"We couldn't setup your command parameters. Exception: {exception.Message}");
            }
        }


        // private void RunTerminalMode()
        // {
        //     var menuItemList = _settings.Menu;
        //     if (!menuItemList.Any())
        //     {
        //         WriteErrorMessage("Please add commands to add functionality.");
        //         return;
        //     }
        //     string input;
        //     do
        //     {
        //         Console.WriteLine($"Available commands:");

        //         menuItemList = menuItemList.OrderBy(x => x.Title).ToList();
        //         menuItemList.ForEach(OutputMenuItem);

        //         Console.Write($"{Environment.NewLine}Command> ");
        //         input = Console.ReadLine() ?? string.Empty;
        //         SetupConsoleErrorColor();
        //         // var commandWithArgs = InputParser.FindCommand(menuItemList, InputParser.ParseInputToArguments(input));
        //         SetupConsoleRunnerColor();
                // if (commandWithArgs.Item1 != null)
                // {
                //     try
                //     {
                //         SetupConsoleCommandColor();
                //         commandWithArgs.Item1.Execute(commandWithArgs.Item2.ToList());
                //     }
                //     catch (Exception exception)
                //     {
                //         WriteErrorMessage(exception.Message);
                //     }
                //     finally
                //     {
                //         SetupConsoleRunnerColor();
                //     }
                //     Console.WriteLine();
                // }
        //     } while (string.IsNullOrEmpty(input) || !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase));
        // }
    }

    public static class ConsoleWrite
    {
        public static void WriteErrorLine(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColor;
        }
    }
}