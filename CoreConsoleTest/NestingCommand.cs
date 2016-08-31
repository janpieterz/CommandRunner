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
    }
}