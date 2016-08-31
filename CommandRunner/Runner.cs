using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public class Runner
    {
        public static void ScanAndStart(string title)
        {
            var reflectionParser = new ReflectionParser();
            var menuItems = reflectionParser.ReflectAllAssemblies();
            Start(title, menuItems);
        }
        public static void ScanAndStart(string title, Func<Type, object> activator)
        {
            var reflectionParser = new ReflectionParser()
                .WithActivator(activator);
            var menuItems = reflectionParser.ReflectAllAssemblies();
            Start(title, menuItems);
        }
        public static void Start(string title, IEnumerable<IMenuItem> menuItems)
        {
            Console.Title = title;
            var args = Environment.GetCommandLineArgs();
            if (args.Length == 0 || (args.FirstOrDefault() == Assembly.GetEntryAssembly().Location))
            {
                StartTerminalMode(menuItems);
            }
            else
            {
                RunCommand(menuItems.OfType<ICommand>(), args);
                Console.WriteLine("Press enter to quit.");
                Console.ReadLine();

            }
        }
        public static void ScanAndRunCommand(IEnumerable<string> arguments)
        {
            var reflectionParser = new ReflectionParser();
            var commands = reflectionParser.ReflectAllAssemblies();
            RunCommand(commands.OfType<ICommand>(), arguments);
        }
        public static void RunCommand(IEnumerable<ICommand> availableCommands,
            IEnumerable<string> arguments)
        {
            var argumentsAsList = arguments.ToList();
            var command = FindCommand(availableCommands, argumentsAsList);
            command.Execute(argumentsAsList);
        }
        public static void StartTerminalMode(IEnumerable<IMenuItem> menuItems)
        {
            var menuItemList = menuItems.ToList();
            if (!menuItemList.Any())
            {
                Console.WriteLine("Please add commands to add functionality.");
                return;
            }
            string input;
            do
            {
                Console.WriteLine($"Available commands:");
                menuItemList = menuItemList.OrderBy(x => x.Title).ToList();
                var groupedMenuItems = menuItemList.GroupBy(x => x.Title.Split(' ')[0]);
                foreach (IGrouping<string, IMenuItem> groupedMenuItem in groupedMenuItems)
                {
                    Console.WriteLine();
                    foreach (IMenuItem menuItem in groupedMenuItem)
                    {
                        if (menuItem is ContainerCommand)
                        {
                            Console.WriteLine($"  {menuItem.Title.ToLowerInvariant()} {menuItem.Help}");
                        }
                        else
                        {
                            Console.WriteLine($"  {menuItem.Title.ToLowerInvariant()}: {menuItem.Help}");
                        }
                    }
                    Console.WriteLine();
                }

                Console.Write($"{Environment.NewLine}Command> ");
                input = Console.ReadLine() ?? string.Empty;
                var inputAsArguments = InputParser.ParseInputToArguments(input).ToList();
                ICommand command = FindCommand(menuItemList.OfType<ICommand>(), inputAsArguments);
                if (command != null)
                {
                    try
                    {
                        command.Execute(inputAsArguments);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                    }
                    Console.WriteLine();
                }
            } while (string.IsNullOrEmpty(input) || !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase));
        }
        private static ICommand FindCommand(IEnumerable<ICommand> commands, IEnumerable<string> arguments)
        {
            var argumentsAsList = arguments.ToList();
            var commandsAsList = commands.ToList();
            var firstArgument = argumentsAsList.FirstOrDefault();
            if (firstArgument == null)
            {
                Console.WriteLine("Please provide a valid command. No input provided.");
                return null;
            }

            string identifier = string.Empty;
            foreach (var argument in argumentsAsList)
            {
                identifier = string.IsNullOrWhiteSpace(identifier) ? argument : $"{identifier} {argument}";
                ICommand command = (ICommand)commandsAsList.FirstOrDefault(x => x.Title.Equals(identifier, StringComparison.OrdinalIgnoreCase));
                if (command != null)
                {
                    return command;
                }
            }
            
            Console.WriteLine($"Please provide a valid command. Input was: {identifier}");
            return null;
        }
    }
}