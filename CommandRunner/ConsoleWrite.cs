using System;

namespace CommandRunner
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

    public static class ErrorMessages
    {
        public static string NoArgumentsProvided = "Please provide an argument";
    }
}