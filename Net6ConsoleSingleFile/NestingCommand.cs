using CommandRunner;

namespace Net6ConsoleSingleFile
{
    [Command("nest", "Commands can nest too.")]
    public class NestingCommand
    {
        [Command("hello", "Say hello")]
        public void Hello()
        {
            Console.WriteLine("Hello");
        }

        [Command("sput")]
        public void Sput(string sput)
        {
            Console.WriteLine(sput);
        }

        [Command("timer")]
        public void Timer(TimeSpan span)
        {
            Console.WriteLine(span);
        }
    }
}