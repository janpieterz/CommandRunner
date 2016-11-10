using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner.CommandLine
{
    internal class CommandLineRunner : IStartableRunner {
        private readonly CommandLineState _state;
        internal CommandLineRunner(CommandLineState state)
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
                HelpPrinter.PrintHelp(_state.Title, _state.Menu);
                Console.ForegroundColor = _state.StartupColor;
                return;
            }
            FindAndExecuteCommand();
        }

        private void FindAndExecuteCommand()
        {
            var matches =
                    _state.Menu.Select(x => new { Key = x, Value = x.Match(_state.Arguments) })
                        .Where(x => x.Value != MatchState.Miss)
                        .ToDictionary(pair => pair.Key, pair => pair.Value);
            if (!matches.Any())
            {
                ConsoleWrite.WriteErrorLine("Please provide a valid command.");
                return;
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
                    var navigatableCommand = match.Key as NavigatableCommand;
                    if (navigatableCommand != null)
                    {
                        ExecuteCommand(match.Key);
                        _state.Arguments = _state.Arguments.Skip(navigatableCommand.Identifier.Split(' ').Length)
                            .Skip(navigatableCommand.MinimumParameters).ToList();
                        FindAndExecuteCommand();
                    }
                    else
                    {
                        ConsoleWrite.WriteErrorLine("Looks like you provided too much parameters for your command:");
                        match.Key.WriteToConsole();
                    }
                    
                }
                else if (match.Value == MatchState.WrongTypes)
                {
                    ConsoleWrite.WriteErrorLine("The provided types did not match the method parameters!");
                }
                else if (match.Value == MatchState.Matched)
                {
                    ExecuteCommand(match.Key);
                }
            }
        }

        private void ExecuteCommand(ICommand command)
        {
            try
            {
                var commandInstance = _state.StatefullCommandActivator(command.Type);
                Console.ForegroundColor = _state.CommandColor;
                var navigatableCommand = command as NavigatableCommand;
                if (command.Parameters.Count > 0)
                {
                    var typedParameters =
                        TypedParameterExecution.CreateTypedParameters(command.Parameters.ToArray(),
                            command.ArgumentsWithoutIdentifier(_state.Arguments));
                    command.MethodInfo.Invoke(commandInstance, typedParameters);

                }
                else
                {
                    //Navigation commands don't always have an initialize method
                    if (navigatableCommand != null)
                    {
                        navigatableCommand.MethodInfo?.Invoke(commandInstance, null);
                    }
                    else
                    {
                        command.MethodInfo.Invoke(commandInstance, null);
                    }
                }

                if (navigatableCommand != null)
                {
                    _state.Menu = navigatableCommand.SubItems;
                    //_state.SetMenu(navigatableCommand.SubItems, navigatableCommand);
                }
            }
            catch (Exception exception)
            {
                ConsoleWrite.WriteErrorLine($"We couldn't setup your command parameters. Exception: {exception.Message}");
            }
            finally
            {
                Console.ForegroundColor = _state.StartupColor;
            }
        }
    }
}