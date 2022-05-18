using CommandRunner;

namespace Net6ConsoleSingleFile
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