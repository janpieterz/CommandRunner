using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandRunner.Terminal;
using CommandRunner.CommandLine;

namespace CommandRunner {
    class InitializableRunner {
        readonly RunnerConfiguration _configuration;
        public InitializableRunner(RunnerConfiguration configuration) {
            _configuration = configuration;
        }

        public IStartableRunner Initialize() {
            ScanTypes();
            
            //TODO: Scan for command dupes or commands with help and throw errors
            //TODO: Scan for IEnumerable that are not the last parameter and throw error
            return CreateRunnerForMode();
        }
        private void SetMode() {
            if (!_configuration.RunMode.HasValue) {
                var args = Environment.GetCommandLineArgs();
                if (args.Length == 0 || (args.FirstOrDefault() == Assembly.GetEntryAssembly().Location || args.FirstOrDefault().Contains("vshost.exe")))
                {
                    _configuration.RunMode = RunModes.Terminal;
                }
                else
                {
                    _configuration.RunMode = RunModes.CommandLine;
                }
            }
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

            _configuration.NavigatableTypes = navigatableTypes;
            _configuration.Menu = commands;
            //GET ALL nested/navigatable
            //PROCESS these
            //GET ALL COMMANDS WITHOUT A NEST/NAV, ensure no nav property contains any of these

        }
        private IStartableRunner CreateRunnerForMode()
        {            
            SetMode();
            if(!_configuration.RunMode.HasValue) {
                throw new Exception("No correct runmode found.");
            }

            if(_configuration.RunMode.Value == RunModes.Terminal) {
                return new TerminalRunner(_configuration);
            }
            if(_configuration.RunMode.Value == RunModes.CommandLine) {
                return new CommandLineRunner(_configuration);
            }
            throw new Exception("No correct runmode found.");
        }

        public NavigatableCommand ProcessNavigatableCommand(Type navigatableCommand, NavigatableCommandAttribute navigatableAttribute, List<Type> scannedTypes)
        {
            scannedTypes.Add(navigatableCommand);
            var initializeMethods =
                navigatableCommand.GetMethods()
                    .Where(x => x.GetCustomAttribute<NavigatableCommandInitialisationAttribute>() != null).ToList();
            if (initializeMethods.Count == 0)
            {
                throw new Exception(
                    $"{navigatableCommand.Name} does not have a method with the attribute {nameof(NavigatableCommandInitialisationAttribute)}");
            }
            if (initializeMethods.Count > 1)
            {
                throw new Exception(
                    $"{navigatableCommand.Name} has multiple methods with the attribute {nameof(NavigatableCommandInitialisationAttribute)}");
            }
            var initializeMethod = initializeMethods.Single();
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
            return new NavigatableCommand
            {
                Identifier = navigatableAttribute.Identifier.Trim().ToLowerInvariant(),
                Help = navigatableAttribute.Help,
                Parameters = initializeMethod.GetParameters().ToList(),
                SubItems = subItems,
                Type = navigatableCommand,
                MethodInfo = initializeMethod
            };
        }
    }

    public interface ICommand
    {
        MethodInfo MethodInfo { get; }
        List<ParameterInfo> Parameters { get; }
        string Identifier { get; }
        void WriteToConsole();
        MatchState Match(List<string> arguments);
        List<string> ArgumentsWithoutIdentifier(List<string> arguments);
        Type Type { get; }

    }
}