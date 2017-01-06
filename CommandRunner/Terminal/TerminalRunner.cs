using System;
using System.Linq;

namespace CommandRunner.Terminal
{
    internal class TerminalRunner : BaseRunner, IStartableRunner {
        private readonly TerminalState _state;
        internal TerminalRunner (TerminalState state) : base(state)
        {
            _state = state;
            Console.Title = _state.Title;
        }
        public void Start()
        {
            string input;
            do
            {
                Console.WriteLine("Setting foreground color to terminal color");
                Console.ForegroundColor = _state.TerminalColor;
                Printer.PrintLine();
                Printer.PrintMenu(_state);
                input = QueryForcommand();
                if (input == null)
                {
                    break;
                }
                var arguments = InputParser.ParseInputToArguments(input).ToList();
                Console.WriteLine();
                if (!arguments.Any())
                {
                    ConsoleWrite.WriteErrorLine(ErrorMessages.NoArgumentsProvided);
                    continue;
                }
                if (arguments[0] == "help")
                {
                    var identifier = input.Split(' ')[1];
                    var item = _state.NavigatableMenu.FirstOrDefault(x => x.Identifier == identifier);
                    if (item != null)
                    {
                        Console.WriteLine("Help for: {0}", item.Identifier);
                        Printer.PrintNavigatableItems(item.SubItems.OfType<NavigatableCommand>().ToList());
                        Printer.PrintSingleCommands(item.SubItems.OfType<SingleCommand>().ToList());
                        
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

                var match = Match(arguments);
                if (match == null) continue;
                if (match.Item2 == MatchState.TooManyArguments)
                {
                    ConsoleWrite.WriteErrorLine(ErrorMessages.TooManyArguments);
                    match.Item1.WriteToConsole();
                }
                else if (match.Item2 == MatchState.Matched)
                {
                    ExecuteCommand(match.Item1, arguments);
                }
            } while (string.IsNullOrEmpty(input) || !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase));
            if (input != null && !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
            {
                Console.ReadLine();
            }
        }

        private string QueryForcommand()
        {
            Console.Write($"{Environment.NewLine}Command> ");
            Console.ForegroundColor = _state.CommandColor;
            var result = Console.ReadLine();
            return result;
        }

        internal override void SetMenu(NavigatableCommand command, object commandInstance)
        {
            _state.SetMenu(command, commandInstance);
        }
    }
}