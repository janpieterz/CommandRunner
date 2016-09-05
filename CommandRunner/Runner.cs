using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{    
    public class Runner
    {
        private readonly RunnerSettings _settings;
        public Runner()
        {
            _settings = new RunnerSettings
            {
                Title = "Command Runner",
                ScanAllAssemblies = true,
                ReflectionActivator = true
            };
        }
        public Runner(Action<ICustomizableRunnerConfiguration> configure)
        {
            if(configure == null) throw new ArgumentNullException(nameof(configure));

            _settings = new RunnerSettings();
            configure(_settings);
        }
        public void Run()
        {
            SetupConsole();
            SetupMenu();
            DeciceMode();
            Execute();
        }

        private void SetupConsole()
        {
            SetupConsoleTitle();
            SetupConsoleRunnerColor();
        }

        private void SetupConsoleTitle()
        {
            if (!string.IsNullOrWhiteSpace(_settings.Title))
            {
                Console.Title = _settings.Title;
            }
        }

        private void SetupConsoleRunnerColor()
        {
            Console.ForegroundColor = _settings.RunnerColor;
        }

        private void SetupConsoleCommandColor()
        {
            Console.ForegroundColor = _settings.CommandColor;
        }

        private void SetupConsoleErrorColor()
        {
            Console.ForegroundColor = _settings.ErrorColor;
        }

        private void SetupMenu()
        {
            if (_settings.Menu.Any())
            {
                return;
            }
            var commandMethods = CommandMethodReflector.GetCommandMethods(_settings)?.ToList();
            var menu = new List<IMenuItem>();
            if (commandMethods == null)
            {
                WriteErrorMessage("Please make sure to setup command scanning or provide your own commands.");
            }
            else if (commandMethods.Count == 0)
            {
                WriteErrorMessage("No commands found.");
                return;
            }
            else
            {
                menu.AddRange(MenuCreator.CreateMenuItems(commandMethods, _settings));
            }
            _settings.Menu = menu;
        }
        private void DeciceMode()
        {
            if (_settings.ForceCommandLine)
            {
                _settings.Mode = RunMode.CommandLine;
            }
            else if (_settings.ForceTerminal)
            {
                _settings.Mode = RunMode.Terminal;
            }
            else
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length == 0 || (args.FirstOrDefault() == Assembly.GetEntryAssembly().Location || args.FirstOrDefault().Contains("vshost.exe")))
                {
                    _settings.Mode = RunMode.Terminal;
                }
                else
                {
                    _settings.Mode = RunMode.CommandLine;
                }
            }
        }
        private void Execute()
        {
            if (_settings.Mode == RunMode.CommandLine)
            {
                RunCommandLineMode();
            }
            else if (_settings.Mode == RunMode.Terminal)
            {
                RunTerminalMode();
            }
        }
        private void RunCommandLineMode()
        {
            var arguments = Environment.GetCommandLineArgs().ToList();
            var commandWithArgs = InputParser.FindCommand(_settings.Menu.OfType<ICommand>(), arguments);
            commandWithArgs.Item1?.Execute(commandWithArgs.Item2.ToList());
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }
        private void RunTerminalMode()
        {
            var menuItemList = _settings.Menu;
            if (!menuItemList.Any())
            {
                WriteErrorMessage("Please add commands to add functionality.");
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
                            Console.WriteLine($"{menuItem.Title.ToLowerInvariant()} {menuItem.Help}");
                        }
                        else
                        {
                            Console.WriteLine($"  {menuItem.Title.ToLowerInvariant()}: {menuItem.Help}");
                        }
                    }
                }

                Console.Write($"{Environment.NewLine}Command> ");
                input = Console.ReadLine() ?? string.Empty;
                SetupConsoleErrorColor();
                var commandWithArgs = InputParser.FindCommand(menuItemList.OfType<ICommand>(), InputParser.ParseInputToArguments(input));
                SetupConsoleRunnerColor();
                if (commandWithArgs.Item1 != null)
                {
                    try
                    {
                        SetupConsoleCommandColor();
                        commandWithArgs.Item1.Execute(commandWithArgs.Item2.ToList());
                    }
                    catch (Exception exception)
                    {
                        WriteErrorMessage(exception.Message);
                    }
                    finally
                    {
                        SetupConsoleRunnerColor();
                    }
                    Console.WriteLine();
                }
            } while (string.IsNullOrEmpty(input) || !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase));
        }

        private void WriteErrorMessage(string error)
        {
            SetupConsoleErrorColor();
            Console.WriteLine(error);
            SetupConsoleRunnerColor();
        }
    }
}