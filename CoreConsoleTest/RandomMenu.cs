using System;

namespace CommandRunner.CoreConsoleTest
{
    [NavigatableCommand("random")]
    public class RandomMenu
    {
        [Command("number")]
        public void RandomNumber()
        {
            Random random = new Random();
            Console.Write(random.Next(1,100));
        }
    }
}