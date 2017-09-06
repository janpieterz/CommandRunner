using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CommandRunner.Tests
{
    public class PublicMethodScanningTests
    {
        [Fact]
        public void DefaultDoesntScanPublicMethods()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.ScanTypes(new List<Type>());
            configuration.AddTypes(new List<Type>(){ typeof(ExamplePublic) });
            Assert.Single(configuration.TypesToScan);
            InitializableRunner runner = new InitializableRunner(configuration);
            runner.Initialize();
            Assert.Empty(runner.Menu);
            Assert.Empty(runner.NavigatableTypes);
        }

        [Fact]
        public void ScanPublicMethods()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.ScanTypes(new List<Type>());
            configuration.AddTypes(new List<Type>() { typeof(ExamplePublic) }, true);
            Assert.Empty(configuration.TypesToScan);
            Assert.Single(configuration.TypesToScanForPublicMethods);
            InitializableRunner runner = new InitializableRunner(configuration);
            runner.Initialize();
            Assert.Single(runner.Menu);
            Assert.Single(runner.NavigatableTypes);
            Assert.Equal("example public", runner.Menu.First().Identifier);
            var navigatableCommand = (NavigatableCommand) runner.Menu.First();
            Assert.Single(navigatableCommand.SubItems);
            Assert.Equal("test method", navigatableCommand.SubItems.First().Identifier);
        }

        public class ExamplePublic
        {
            public void TestMethod()
            {
                
            }

            public string Tester { get; set; }
        }
    }
}
