using System;

namespace CommandRunner.CoreConsoleTest
{
    [Command("nest", "Commands can nest too.")]
    public class NestingCommand
    {
        [Command("hello", "Say hello")]
        public void Hello()
        {
            Console.WriteLine("Hello");
        }

        [Command("hi", "Say hi")]
        public void SayHi()
        {
            Console.WriteLine("Hi");
        }

        [Command("hi", "Say hello hi double dupe")]
        public void SayHiAgain()
        {
            Console.WriteLine(":O");
        }
    }
}