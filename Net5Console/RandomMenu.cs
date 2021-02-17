using System;

namespace CommandRunner.Net5Console
{
    [NavigatableCommand("random")]
    public class RandomMenu
    {
        [Command("number")]
        public void RandomNumber()
        {
            Random random = new Random();
            Console.WriteLine(random.Next(1,100));
        }
    }
}