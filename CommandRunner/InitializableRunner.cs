using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandRunner.Terminal;
using CommandRunner.CommandLine;

namespace CommandRunner {
    internal class InitializableRunner {

        readonly RunnerConfiguration _configuration;
        internal List<Type> NavigatableTypes { get; set; }
        internal List<ICommand> Menu { get; set; }
        internal List<string> Arguments { get; set; }
        internal RunModes Mode { get; set; }
        internal ICommandScanner CommandScanner { get; set; }
        public InitializableRunner(RunnerConfiguration configuration) {
            _configuration = configuration;
            CommandScanner = new CommandScanner();
        }

        internal IStartableRunner Initialize() {
            var scanResult = CommandScanner.ScanTypes(_configuration.TypesToScan);
            var publicScanResult = CommandScanner.ScanTypesForPublicMethods(_configuration.TypesToScanForPublicMethods);
            Menu = scanResult.Item1;
            NavigatableTypes = scanResult.Item2;
            Menu.AddRange(publicScanResult.Item1);
            NavigatableTypes.AddRange(publicScanResult.Item2);

            SetArguments();
            RemoveRedundantArguments();
            SetMode();
            ValidateSettings();
            return CreateRunnerForMode();
        }

        private void ValidateSettings()
        {
            Validator.ValidateDuplicates(Menu);
            Validator.ValidateHelp(Menu);
            Validator.ValidateEnumerable(Menu);
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
                var entryAssembly = Assembly.GetEntryAssembly();
                if (Arguments.Count == 0 ||
                    (entryAssembly != null && (Arguments.FirstOrDefault() == entryAssembly.Location ||
                     Arguments.FirstOrDefault().Contains("vshost.exe"))&& Arguments.Count == 1))
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
            var entryAssembly = Assembly.GetEntryAssembly();

            //https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file/overview#api-incompatibility
            var args = Environment.GetCommandLineArgs();
            var processName = args.Length > 0 ? args[0] : null;
            if (!string.IsNullOrEmpty(processName))
            {
                Arguments.Remove(processName);
            }

            if (entryAssembly != null && !string.IsNullOrEmpty(entryAssembly.Location)) //https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file/overview#api-incompatibility
            {
                Arguments.Remove(entryAssembly.Location);
                Arguments.Remove(entryAssembly.Location.Replace("\\", "/"));

                var exeVersion = entryAssembly.Location.Substring(0, entryAssembly.Location.Length - 4) + ".exe";
                
                Arguments.Remove(exeVersion);
                Arguments.Remove(exeVersion.Replace("\\", "/"));
            }
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
            state.FullMenu = Menu;
            state.ActiveMenu = Menu;
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