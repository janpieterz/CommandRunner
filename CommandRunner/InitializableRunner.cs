using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandRunner.Terminal;
using CommandRunner.CommandLine;

namespace CommandRunner {
    internal class InitializableRunner {
        readonly RunnerConfiguration _configuration;
        private List<Type> NavigatableTypes { get; set; }
        private List<ICommand> Menu { get; set; }
        private List<string> Arguments { get; set; }
        private RunModes Mode { get; set; }
        public InitializableRunner(RunnerConfiguration configuration) {
            _configuration = configuration;
        }

        internal IStartableRunner Initialize() {
            ScanTypes();
            SetArguments();
            SetMode();
            RemoveRedundantArguments();
            ValidateSettings();
            return CreateRunnerForMode();
        }

        private void ValidateSettings()
        {
            ValidateDuplicates(Menu);
            ValidateHelp(Menu);
            ValidateEnumerable(Menu);
        }

        private void ValidateEnumerable(List<ICommand> commands)
        {
            List<string> wrongEnumerableCommands = FindWrongEnumerables(commands);
            if (wrongEnumerableCommands.Any())
            {
                throw new Exception(
                    string.Format(
                        "We've found occurances of an enumerable being used at a wrong position. Enumerables can only be used as the last parameter. Commands: [{0}]",
                        string.Join(",", wrongEnumerableCommands)));
            }
        }

        private List<string> FindWrongEnumerables(List<ICommand> commands)
        {
            List<string> items = new List<string>();
            var wrongEnumerables =
                commands.Where(
                    x =>
                        x.Parameters.Take(x.Parameters.Count - 1)
                            .Any(parameter => parameter.ParameterType.IsEnumerable()));
            items.AddRange(wrongEnumerables.Select(x => $"{x.Type.Name}:{x.Identifier}"));

            foreach (NavigatableCommand navigatableCommand in commands.OfType<NavigatableCommand>())
            {
                items.AddRange(FindWrongEnumerables(navigatableCommand.SubItems));
            }
            return items;
        }

        private void ValidateHelp(List<ICommand> commands)
        {
            List<string> helpCommands = FindHelpCommands(commands);
            if (helpCommands.Any())
            {
                throw new Exception(string.Format("You have commands that start the prohibited `help` keywords (`help`, `/?`, `-h`). Commands: [{0}]", string.Join(",", helpCommands)));
            }
        }

        private List<string> FindHelpCommands(List<ICommand> commands)
        {
            List<string> items = new List<string>();
            var firstLevelHelps =
                commands.Where(
                    x => x.Identifier.StartsWith("help") || x.Identifier.StartsWith("/?") || x.Identifier.StartsWith("-h"));
            items.AddRange(firstLevelHelps.Select(x => $"{x.Type.Name}:{x.Identifier}"));

            foreach (NavigatableCommand navigatableCommand in commands.OfType<NavigatableCommand>())
            {
                items.AddRange(FindHelpCommands(navigatableCommand.SubItems));
            }
            return items;
        }

        private void ValidateDuplicates(List<ICommand> commands)
        {
            List<string> dupes = FindDuplicates(commands);
            if (dupes.Any())
            {
                throw new Exception(string.Format("You have duplicate commands. Commands: [{0}]", string.Join(",", dupes)));
            }
        }

        private List<string> FindDuplicates(List<ICommand> commands)
        {
            List<string> items = new List<string>();
            var firstLevelDuplicates = commands.GroupBy(x => x.Identifier).Where(x => x.Count() > 1).ToList();
            foreach (IGrouping<string, ICommand> grouping in firstLevelDuplicates)
            {
                foreach (ICommand command in grouping)
                {
                    items.Add($"{command.Type.Name}:{command.Identifier}");
                }
            }
            
            foreach (NavigatableCommand navigatableCommand in commands.OfType<NavigatableCommand>())
            {
                items.AddRange(FindDuplicates(navigatableCommand.SubItems));
            }
            return items;
        }

        private void ScanTypes()
        {
            var nestedCommands = _configuration.TypesToScan.Where(x =>
            {
                var typeInfo = x.GetTypeInfo();
                return typeInfo.GetCustomAttribute<NestedCommandAttribute>() != null;
            });
            List<ICommand> commands = new List<ICommand>();
            foreach (var nestedCommand in nestedCommands)
            {
                var nestedAttribute = nestedCommand.GetTypeInfo().GetCustomAttribute<NestedCommandAttribute>();
                var nestedMethods = nestedCommand.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
                foreach (MethodInfo methodInfo in nestedMethods)
                {
                    var methodAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                    var parameters = methodInfo.GetParameters();
                    
                    var command = new SingleCommand()
                    {
                        Identifier = $"{nestedAttribute.Identifier.Trim().ToLowerInvariant()} {methodAttribute.Identifier.Trim().ToLowerInvariant()}",
                        Help = methodAttribute.Help,
                        Parameters = parameters.ToList(),
                        MethodInfo = methodInfo,
                        Type = nestedCommand
                    };
                    commands.Add(command);
                }
            }

            var navigatableCommands =
                _configuration.TypesToScan.Where(
                    x => x.GetTypeInfo().GetCustomAttribute<NavigatableCommandAttribute>() != null);
            var navigatableTypes = new List<Type>();
            foreach (Type navigatableCommand in navigatableCommands)
            {
                var navigatableAttribute = navigatableCommand.GetTypeInfo().GetCustomAttribute<NavigatableCommandAttribute>();
                var menuItem = ProcessNavigatableCommand(navigatableCommand, navigatableAttribute, navigatableTypes);
                commands.Add(menuItem);
            }

            var typesLeft = _configuration.TypesToScan.Where(
                x =>
                    navigatableTypes.All(y => y != x) &&
                    x.GetTypeInfo().GetCustomAttribute<NestedCommandAttribute>() == null);
            foreach (Type type in typesLeft)
            {
                var methods = type.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
                foreach (MethodInfo methodInfo in methods)
                {
                    var methodAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                    commands.Add(new SingleCommand()
                    {
                        Identifier = methodAttribute.Identifier.Trim().ToLowerInvariant(),
                        Help = methodAttribute.Help,
                        Parameters = methodInfo.GetParameters().ToList(),
                        Type = type,
                        MethodInfo = methodInfo
                    });
                }
            }

            NavigatableTypes = navigatableTypes;
            Menu = commands;

        }

        internal NavigatableCommand ProcessNavigatableCommand(Type navigatableCommand, NavigatableCommandAttribute navigatableAttribute, List<Type> scannedTypes)
        {
            scannedTypes.Add(navigatableCommand);
            
            var subItems = new List<ICommand>();

            var commandMethods =
                navigatableCommand.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
            foreach (MethodInfo commandMethod in commandMethods)
            {
                var methodAttribute = commandMethod.GetCustomAttribute<CommandAttribute>();
                var parameters = commandMethod.GetParameters();

                var command = new SingleCommand()
                {
                    Identifier = methodAttribute.Identifier.Trim().ToLowerInvariant(),
                    Help = methodAttribute.Help,
                    Parameters = parameters.ToList(),
                    Type = navigatableCommand,
                    MethodInfo = commandMethod
                };
                subItems.Add(command);
            }
            var navigatableSubCommands =
                navigatableCommand.GetProperties()
                    .Where(x => x.GetCustomAttribute<NavigatableCommandAttribute>() != null);

            foreach (PropertyInfo navigatableSubCommand in navigatableSubCommands)
            {
                scannedTypes.Add(navigatableSubCommand.PropertyType);
                subItems.Add(ProcessNavigatableCommand(navigatableSubCommand.PropertyType, navigatableSubCommand.GetCustomAttribute<NavigatableCommandAttribute>(), scannedTypes));
            }

            var navigatableCommandMethods = navigatableCommand.GetMethods();
            var initializeMethods =
                navigatableCommandMethods
                    .Where(x => x.GetCustomAttribute<NavigatableCommandInitialisationAttribute>() != null).ToList();

            var announceMethods =
                navigatableCommandMethods.Where(
                    x => x.GetCustomAttribute<NavigatableCommandAnnouncementAttribute>() != null).ToList();

            if (initializeMethods.Count > 1)
            {
                throw new Exception(
                    $"{navigatableCommand.Name} has multiple methods with the attribute {nameof(NavigatableCommandInitialisationAttribute)}");
            }
            if (announceMethods.Count > 1)
            {
                throw new Exception(
                    $"{navigatableCommand.Name} has multiple methods with the attribute {nameof(NavigatableCommandAnnouncementAttribute)}");
            }
            var initializeMethod = initializeMethods.SingleOrDefault();
            var announceMethod = announceMethods.SingleOrDefault();
            return new NavigatableCommand
            {
                Identifier = navigatableAttribute.Identifier.Trim().ToLowerInvariant(),
                Help = navigatableAttribute.Help,
                Parameters = initializeMethod?.GetParameters().ToList(),
                SubItems = subItems,
                Type = navigatableCommand,
                MethodInfo = initializeMethod,
                AnnounceMethod = announceMethod
            };
        }

        private void SetArguments()
        {
            var arguments = _configuration.Arguments != null && _configuration.Arguments.Any()
                ? _configuration.Arguments
                : Environment.GetCommandLineArgs().ToList();
            Arguments = arguments;
        }

        private void SetMode() {
            if (!_configuration.RunMode.HasValue)
            {
                if (Arguments.Count == 0 ||
                    (Arguments.FirstOrDefault() == Assembly.GetEntryAssembly().Location ||
                     Arguments.FirstOrDefault().Contains("vshost.exe")))
                {
                    Mode = RunModes.Terminal;
                }
                else
                {
                    Mode = RunModes.CommandLine;
                }
            }
            else
            {
                Mode = _configuration.RunMode.Value;
            }
        }

        private void RemoveRedundantArguments()
        {
            Arguments.Remove(Assembly.GetEntryAssembly().Location);
            var firstArgument = Arguments.FirstOrDefault();
            if (!string.IsNullOrEmpty(firstArgument) && firstArgument.Contains("vshost.exe"))
            {
                Arguments.Remove(Arguments.FirstOrDefault());
            }
        }


        private IStartableRunner CreateRunnerForMode()
        {   
            if(Mode == RunModes.Terminal) {
                var state = new TerminalState();
                SetupState(state);
                return new TerminalRunner(state);
            }
            if(Mode == RunModes.CommandLine) {
                var state = new CommandLineState();
                SetupState(state);
                return new CommandLineRunner(state);
            }
            throw new Exception("No correct runmode found.");
        }

        private void SetupState(State state)
        {
            state.Menu = Menu;
            state.CommandActivator = _configuration.CommandActivator;
            state.Arguments = Arguments;
            state.CommandColor = _configuration.CommandColor;
            state.NavigatableTypes = NavigatableTypes;
            state.RunMode = Mode;
            state.TerminalColor = _configuration.TerminalColor;
            state.Title = _configuration.Title;
            state.StartupColor = Console.ForegroundColor;
        }
    }
}