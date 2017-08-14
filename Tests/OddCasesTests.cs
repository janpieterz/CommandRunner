using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CommandRunner.Terminal;
using Moq;
using Xunit;

namespace CommandRunner.Tests
{
    public class OddCasesTests
    {
        [Theory, InlineData]
        public void TestAsyncVoidReturns()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.TypesToScan.Add(typeof(AsyncVoidTest));
            configuration.ForceTerminal();
            Mock<IDisposable> actionTracker = new Mock<IDisposable>();
            AsyncVoidTest testTracker = new AsyncVoidTest()
            {
                ActionTracker = actionTracker.Object
            };
            configuration.UseCommandActivator(x =>
            {
                if (x == typeof(AsyncVoidTest))
                {
                    return testTracker;
                }
                return null;
            });
            var runner = (TerminalRunner)Runner.Create(configuration);
            var result = runner.ExecuteCommand(runner.State.ActiveMenu.Single(x => x.Identifier == "test"), new List<string>());
            Assert.False(result);
        }
        [Theory, InlineData()]
        public void TestDuplicateLongNameResolution()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.TypesToScan.Add(typeof(DuplicateNestingCommand));
            configuration.ForceTerminal();

            Assert.Throws<Exception>(() =>
            {
                Runner.Create(configuration);
            });
        }

        [Theory, InlineData()]
        public void TestNestedNameResolution()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.TypesToScan.Add(typeof(NestedNameCommand));
            configuration.ForceTerminal();

            var runner = (TerminalRunner)Runner.Create(configuration);

            var result = runner.Match(new List<string>() { "nested", "name" });
            Assert.NotNull(result);
            Assert.Equal("nested name", result.Item1.Identifier);
        }
        [Theory, InlineData()]
        public void TestNestedNameDeepResolution()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.TypesToScan.Add(typeof(NestedNameCommand));
            configuration.ForceTerminal();

            var runner = (TerminalRunner)Runner.Create(configuration);

            var result = runner.Match(new List<string>() { "nested", "name", "example" });
            Assert.NotNull(result);
            Assert.Equal("nested name example", result.Item1.Identifier);
        }

        public class AsyncVoidTest {

            public IDisposable ActionTracker { get; set; }
            [Command("test")]
            public async void Test()
            {
                await Task.Delay(500).ConfigureAwait(false);
                ActionTracker.Dispose();
            }
        }

        public class DuplicateNestingCommand
        {
            [Command("duplicate nesting")]
            public void DuplicateNesting()
            {

            }

            [Command("duplicate nesting")]
            public void DuplicateNestingExample()
            {

            }
        }

        public class NestedNameCommand
        {
            [Command("nested name")]
            public void DuplicateNesting()
            {

            }

            [Command("nested name example")]
            public void DuplicateNestingExample()
            {

            }
        }
    }
}
