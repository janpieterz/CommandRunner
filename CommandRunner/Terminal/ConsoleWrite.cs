using System;

namespace CommandRunner.Terminal
{
    public static class ConsoleWrite
    {
        public static void WriteErrorLine(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColor;
        }
    }
}