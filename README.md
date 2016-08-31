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