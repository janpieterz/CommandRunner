using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner
{
    public class InputParser
    {
        public static Tuple<ICommand, IEnumerable<string>>FindCommand(IEnumerable<IMenuItem> menuItems, IEnumerable<string> arguments)
        {
            var argumentsAsList = arguments.ToList();
            var commandsAsList = ParseMenuItemsToCommands(menuItems).ToList();

            var firstArgument = argumentsAsList.FirstOrDefault();
            if (firstArgument == null)
            {
                Console.WriteLine("Please provide a valid command. No input provided.");
                return new Tuple<ICommand, IEnumerable<string>>(null, null);
            }

            string identifier = string.Empty;
            foreach (var argument in argumentsAsList)
            {
                identifier = string.IsNullOrWhiteSpace(identifier) ? argument : $"{identifier} {argument}";
                IMenuItem command = commandsAsList.FirstOrDefault(x => x.Title.Equals(identifier, StringComparison.OrdinalIgnoreCase));
                
                if (command != null)
                {
                    return new Tuple<ICommand, IEnumerable<string>>(command as ICommand, argumentsAsList.Skip(identifier.Split(' ').Length));
                }
            }

            Console.WriteLine($"Please provide a valid command. Input was: {identifier}");
            return new Tuple<ICommand, IEnumerable<string>>(null, null);
        }

        public static IEnumerable<ICommand> ParseMenuItemsToCommands(IEnumerable<IMenuItem> menuItems)
        {
            var commandsAsList = menuItems.ToList();

            foreach (ContainerCommand command in commandsAsList.OfType<ContainerCommand>().ToList())
            {
                commandsAsList.AddRange(command.Items);
                commandsAsList.Remove(command);
            }
            return commandsAsList.Cast<ICommand>();
        }

        public static IEnumerable<string> ParseInputToArguments(string input)
        {
            List<string> arguments = new List<string>();
            string buffer = string.Empty;
            bool enclosedProcessing = false;
            foreach (char c in input)
            {
                if (c == ' ' && !enclosedProcessing)
                {
                    arguments.Add(buffer);
                    buffer = string.Empty;
                }
                else if (c == '"')
                {
                    enclosedProcessing = !enclosedProcessing;
                }
                else
                {
                    buffer = buffer + c;
                }
            }
            if (!string.IsNullOrWhiteSpace(buffer))
            {
                arguments.Add(buffer);
            }
            return arguments;
        }
    }
}