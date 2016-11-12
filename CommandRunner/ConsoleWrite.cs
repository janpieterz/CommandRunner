using System;

namespace CommandRunner
{
    internal static class ConsoleWrite
    {
        internal static void WriteErrorLine(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColor;
        }
    }

    internal static class ErrorMessages
    {
        internal static string NoArgumentsProvided = "Please provide an argument";
    }
}