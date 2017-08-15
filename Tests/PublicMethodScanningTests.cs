using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CommandRunner.Tests
{
    public class PublicMethodScanningTests
    {
        [Theory, InlineData]
        public void DefaultDoesntScanPublicMethods()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.ScanTypes(new List<Type>());
            configuration.AddTypes(new List<Type>(){ typeof(ExamplePublic) });
            Assert.Equal(1, configuration.TypesToScan.Count);
            InitializableRunner runner = new InitializableRunner(configuration);
            runner.Initialize();
            Assert.Equal(0, runner.Menu.Count);
            Assert.Equal(0, runner.NavigatableTypes.Count);
        }

        [Theory, InlineData]
        public void ScanPublicMethods()
        {
            RunnerConfiguration configuration = new RunnerConfiguration();
            configuration.ScanTypes(new List<Type>());
            configuration.AddTypes(new List<Type>() { typeof(ExamplePublic) }, true);
            Assert.Equal(0, configuration.TypesToScan.Count);
            Assert.Equal(1, configuration.TypesToScanForPublicMethods.Count);
            InitializableRunner runner = new InitializableRunner(configuration);
            runner.Initialize();
            Assert.Equal(1, runner.Menu.Count);
            Assert.Equal(1, runner.NavigatableTypes.Count);
            Assert.Equal("example public", runner.Menu.First().Identifier);
            var navigatableCommand = (NavigatableCommand) runner.Menu.First();
            Assert.Equal(1, navigatableCommand.SubItems.Count);
            Assert.Equal("test method", navigatableCommand.SubItems.First().Identifier);
        }

        public class ExamplePublic
        {
            public void TestMethod()
            {
                
            }
        }
    }
}
