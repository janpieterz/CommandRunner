using System;
using System.Linq;
using System.Reflection;
using CommandRunner.Terminal;
using CommandRunner.CommandLine;
namespace CommandRunner {
    class InitializableRunner {
        RunnerConfiguration Configuration;
        public InitializableRunner(RunnerConfiguration configuration) {
            Configuration = configuration;
        }

        public IStartableRunner Initialize() {
            ScanTypes();
            //TODO: Scan and setup commands/menu
            //TODO: Setup Console writer with color
            return CreateRunnerForMode();
        }
        private void DecideMode() {
            if (!Configuration.RunMode.HasValue) {
                var args = Environment.GetCommandLineArgs();
                if (args.Length == 0 || (args.FirstOrDefault() == Assembly.GetEntryAssembly().Location || args.FirstOrDefault().Contains("vshost.exe")))
                {
                    Configuration.RunMode = RunModes.Terminal;
                }
                else
                {
                    Configuration.RunMode = RunModes.CommandLine;
                }
            }
        }

        private void ScanTypes() {
            
        }
        private IStartableRunner CreateRunnerForMode()
        {            
            DecideMode();
            if(!Configuration.RunMode.HasValue) {
                throw new Exception("No correct runmode found.");
            }

            if(Configuration.RunMode.Value == RunModes.Terminal) {
                return new TerminalRunner(Configuration);
            }
            else if(Configuration.RunMode.Value == RunModes.CommandLine) {
                return new CommandLineRunner(Configuration);
            }
            else {
                throw new Exception("No correct runmode found.");
            }
        }
    }
}