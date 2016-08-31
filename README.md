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

For more usage refer to the CoreConsoleTest app where you should be able to easily play around with all concepts and options.

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

## Using provided assemblies

```c#
new Runner(options =>
{
    options.Scan.SpecificAssemblies(new List<Assembly>() {Assembly.GetEntryAssembly()});
}).Run();
```

## Using provided types

```c#
new Runner(options =>
{
    options.Scan.SpecificTypes(new List<Type>() { typeof(EchoCommand)});
}).Run();
```

## Using custom menu

```
new Runner(options =>
{
    options.UseMenuItems(new List<IMenuItem>()
    {
        new ReflectedMethodCommand("reflect", "Custom reflected method command", GetTestMethodInfo()),
        new CustomActivatorCommand("activator", "Custom activator method command", GetTestMethodInfo(), type => new NestingCommand())
    }); 
}).Run();
```
