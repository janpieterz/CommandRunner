using System;
using System.Collections.Generic;
using CommandRunner;

namespace CommandRunnerCoreTest
{
    public class EchoCommand : ICommand
    {
        public string Title => "Echo";
        public string Help => "Echo back anything following the command.";

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
}
