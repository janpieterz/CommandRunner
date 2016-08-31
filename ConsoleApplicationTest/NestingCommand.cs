using System;
using CommandRunner;
namespace CommandRunner.ConsoleApplicationTest
{
    [Command("nest", "Commands can nest too.")]
    public class NestingCommand
    {
        [Command("hello", "Say hello")]
        public void Hello()
        {
            Console.WriteLine("Hello");
        }
    }
}