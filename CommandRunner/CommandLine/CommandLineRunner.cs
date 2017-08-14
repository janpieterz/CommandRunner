using System;
using System.Linq;

namespace CommandRunner.CommandLine
{
    internal class CommandLineRunner : BaseRunner, IStartableRunner {
        private readonly CommandLineState _state;
        internal CommandLineRunner(CommandLineState state) : base(state)
        {
            _state = state;
        }
        public void Start()
        {
            if (!_state.Arguments.Any())
            {
                ConsoleWrite.WriteErrorLine(ErrorMessages.NoArgumentsProvided);
                return;
            }
            if (_state.InHelpMode)
            {
                Console.ForegroundColor = _state.TerminalColor;
                HelpPrinter.PrintHelp(_state.Title, _state.FullMenu);
                Console.ForegroundColor = _state.StartupColor;
                return;
            }
            FindAndExecuteCommand();
        }

        private void FindAndExecuteCommand()
        {
            var match = Match(_state.Arguments);
            if (match == null) return;
            if (match.Item1 is NavigatableCommand navigatableCommand)
            {
                var result = ExecuteCommand(match.Item1, _state.Arguments);
                if (result)
                {
                    _state.Arguments = _state.Arguments.Skip(navigatableCommand.Identifier.Split(' ').Length)
                        .Skip(navigatableCommand.MinimumParameters).ToList();
                    if (_state.Arguments.Any())
                    {
                        FindAndExecuteCommand();
                    }
                    else
                    {
                        ConsoleWrite.WriteErrorLine("We seem to be missing a follow up command.");
                    }
                }
                else
                {
                    Environment.Exit(1);
                }
            }
            else
            {
                if (match.Item2 == MatchState.TooManyArguments)
                {
                    ConsoleWrite.WriteErrorLine(ErrorMessages.TooManyArguments);
                    match.Item1.WriteToConsole();
                }
                else if (match.Item2 == MatchState.Matched)
                {
                    var result = ExecuteCommand(match.Item1, _state.Arguments);
                    if (!result)
                    {
                        Environment.Exit(1);
                    }
                }
            }
        }

        internal override void SetMenu(NavigatableCommand command, object commandInstance)
        {
            State.ActiveMenu = command.SubItems;
            State.ParentHierarchy[command.Type] = new ParentCommand()
            {
                Command = command,
                Instance = commandInstance
            };
        }
    }
}