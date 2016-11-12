# CommandRunner
A simple command runner to enable quick command line (developer) tools

![demo](https://github.com/janpieterz/CommandRunner/blob/master/GifCommandRunner.gif|alt=demo)

# Simple Usage
```c#
static void Main(string[] args)
{
	RunnerConfiguration configuration = new RunnerConfiguration("Example Runner");
	Runner.Start(configuration);
}

public class EchoCommand 
{

	[Command("echo", "Echo back anything following the command.")]
	public void Execute(List<string> args)
	{
		foreach (var arg in args) Console.WriteLine(arg);
	}
}

[NestedCommand("nest")]
public class NestingCommand
{
	[Command("hello", "Say hello")]
	public void Hello()
	{
		Console.WriteLine("Hello");
	}
}
```
The library accepts typed parameters and is able to easily setup a quick command line tool.
The library will try to map the arguments if it sees typed arguments. To prevent this, accept a list of strings in your method to parse it yourself, or no parameter at all if nothing is needed.

# Attributes used to setup your runner(make sure to check the CoreconsoleTest app for examples):
```c#
[Command("identifier", "help")]
```
This attribute (used on methods) signals the runner it can run this method using the identifier. Can be used in a NestedCommand and a NavigatableCommand
```c#
[NestedCommand("pre-identifier", "help")]
```
This attribute, used on a class is more for easy prepending of all commands. All methods in the class that use the Command attribute will have their identifier prepended like: '{NestedIdentifier} {CommandIdentifier}'.
```c#
[NavigatableCommand("identifier", "help")]
```
This attribute, used on a class, signals a menu that can be used to navigate into. Child items won't be visible until navigated upon. If there is a multi-layer menu, the parent Menu class will be set on the child if there is a property of that type.
```c#
[NavigatableCommandAnnouncement]
```
This attribute, used on a method, will be called in the terminal mode instead of displaying the identifier when navigated into the command.

```c#
[NavigatableCommandInitialization]
```
This attribute, used on a method, will be called when navigating to a command. This can be used to setup the menu environment, for example an account menu with a specific account id to then only execute every method on this instance. The Initialize method is able to accept typed parameters like other command methods.


# Settings

## Using custom creator

```c#
private static void RunWithAutofacCreator()
{
	var container = CreateContainer();
	var container = CreateContainer();
	RunnerConfiguration configuration = new RunnerConfiguration("Example Runner");
	configuration.UseCommandActivator(type => container.Resolve(type));
	Runner.Start(configuration);
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
configuration.ScanAssemblies(new List<Assembly>() {typeof(EchoCommand).GetTypeInfo().Assembly});
```

## Using provided types

```c#
configuration.ScanTypes(new List<Type>() {typeof(EchoCommand), typeof(AccountMenu)});
```

## Setting specific colors:
```c#
configuration.UseTerminalColor(ConsoleColor.DarkGreen);
configuration.UseCommandColor(ConsoleColor.Yellow);
```

## Forcing mode
```c#
configuration.ForceTerminal();
configuration.ForceCommandLine();
```

## Provide arguments (only applicable for command line mode)
```c#
configuration.UseArguments(new List<string>() {"test", "different", "arguments"});
```
