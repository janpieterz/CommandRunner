using System;
using System.Collections.Generic;
using CommandRunner;

namespace CommandRunnerCoreTest
{
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

    public class KevCommand
    {
        [Command("kev says hi", "Kev is saying hi")]
        public void TestingStuff()
        {
            Console.WriteLine("Kev is saying hi");
        }
    }
}
