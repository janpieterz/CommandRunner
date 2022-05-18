// See https://aka.ms/new-console-template for more information

using CommandRunner;
using Net6ConsoleSingleFile;

var envArgs = Environment.GetCommandLineArgs();
RunnerConfiguration configuration = new RunnerConfiguration("Example Runner");
configuration.ScanTypes(new List<Type>() { typeof(EchoCommand), typeof(NestingCommand), typeof(RandomMenu) });
configuration.AddTypes(new List<Type>(){ typeof(ExamplePublic)}, true);
Runner.Start(configuration);