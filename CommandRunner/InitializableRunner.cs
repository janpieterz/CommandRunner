using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandRunner.Terminal;
using CommandRunner.CommandLine;

namespace CommandRunner {
    class InitializableRunner {
        readonly RunnerConfiguration _configuration;
        private List<Type> NavigatableTypes { get; set; }
        private List<ICommand> Menu { get; set; }
        private List<string> Arguments { get; set; }
        private RunModes Mode { get; set; }
        public InitializableRunner(RunnerConfiguration configuration) {
            _configuration = configuration;
        }

        public IStartableRunner Initialize() {
            ScanTypes();
            SetArguments();
            SetMode();
            RemoveRedundantArguments();
            //TODO: Scan for command dupes or commands with help and throw errors
            //TODO: Scan for IEnumerable that are not the last parameter and throw error
            return CreateRunnerForMode();
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

        public NavigatableCommand ProcessNavigatableCommand(Type navigatableCommand, NavigatableCommandAttribute navigatableAttribute, List<Type> scannedTypes)
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