using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandRunner.Terminal;
using Xunit;

namespace CommandRunner.Tests
{
    public class CommandBehaviorTests
    {

        [Theory, InlineData()]
        public void TestNormalExecution()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.TypesToScan.Add(typeof(Commands));
            configuration.ForceTerminal();

            var runner = (TerminalRunner)Runner.Create(configuration);
            runner.ExecuteCommand(runner.State.NavigatableMenu.Single(x => x.Identifier == "menu"), new List<string>());
            Assert.Equal(3, runner.State.ActiveMenu.Count);
            var result = runner.ExecuteCommand(runner.State.ActiveMenu.Single(x => x.Identifier == "normal"), new List<string>());
            Assert.True(result);
        }
        [Theory, InlineData()]
        public void TestThrowingStaysInMenuExecution()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.TypesToScan.Add(typeof(Commands));
            configuration.ForceTerminal();

            var runner = (TerminalRunner)Runner.Create(configuration);
            runner.ExecuteCommand(runner.State.NavigatableMenu.Single(x => x.Identifier == "menu"), new List<string>());
            Assert.Equal(3, runner.State.ActiveMenu.Count);
            var result = runner.ExecuteCommand(runner.State.ActiveMenu.Single(x => x.Identifier == "throws"), new List<string>());
            Assert.False(result);
            Assert.Equal(3, runner.State.ActiveMenu.Count);
        }
        [Theory, InlineData()]
        public void TestMoveUpExecution()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.TypesToScan.Add(typeof(Commands));
            configuration.ForceTerminal();

            var runner = (TerminalRunner)Runner.Create(configuration);
            runner.ExecuteCommand(runner.State.NavigatableMenu.Single(x => x.Identifier == "menu"), new List<string>());
            Assert.Equal(3, runner.State.ActiveMenu.Count);
            var result = runner.ExecuteCommand(runner.State.ActiveMenu.Single(x => x.Identifier == "goes"), new List<string>());
            Assert.True(result);
            Assert.Equal(1, runner.State.ActiveMenu.Count);
        }

        [NavigatableCommand("menu")]
        public class Commands
        {
            [Command("normal")]
            public void NormalExecution()
            {
                
            }

            [Command("goes", moveUpAfterSuccessfulExecution:true)]
            public void GoesUp()
            {
                
            }

            [Command("throws", moveUpAfterSuccessfulExecution: true)]
            public void DoesNotGoUp()
            {
                throw new Exception();
            }
        }
    }
}
