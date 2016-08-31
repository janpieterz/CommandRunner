# CommandRunner
A simple command runner to enable quick command line (developer) tools
The runner is still in development mode so there will be breaking changes.

# Usage
```c#
static void Main(string[] args)
{
    new Runner.Run();
}

public class EchoCommand 
{

    [Command("echo", "Echo back anything following the command.")]
    public void Execute(List<string> args)
    {
        foreach (var arg in args) Console.WriteLine(arg);
    }
}

[Command("nest")]
public class NestingCommand
{
    [Command("hello", "Say hello")]
    public void Hello()
    {
        Console.WriteLine("Hello");
    }
}
```

# Advanced usage

## Using custom creator

```c#
private static void RunWithAutofacCreator()
{
    var container = CreateContainer();
    new Runner(options =>
    {
        options.Title = "Command Runner";
        options.Scan.AllAssemblies();
        options.Activate.WithCustomActivator(type => container.Resolve(type));
    }).Run();
}

private static IContainer CreateContainer()
{
    var builder = new ContainerBuilder();
    builder.RegisterType<EchoCommand>().PropertiesAutowired();
    builder.RegisterType<Injectable>().PropertiesAutowired();
    builder.RegisterType<NestingCommand>().PropertiesAutowired();
    return builder.Build();
}
```
