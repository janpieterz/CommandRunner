using System;

namespace CommandRunner.CoreConsoleTest
{
    [NestedCommand("nest")]
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
        [Command("throw exception")]
        public void ThrowException()
        {
            throw new Exception("Exception thrown!");
        }
    }
}