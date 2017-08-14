using System;
using System.Collections.Generic;
using CommandRunner.Terminal;
using Xunit;

namespace CommandRunner.Tests
{
    public class OddCasesTests
    {
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
