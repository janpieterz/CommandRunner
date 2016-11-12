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
        internal static string NoMatch = "Please provide a valid command.";
        internal static string MissingArgument = "Make sure you provide all the arguments for your command:";
        internal static string TooManyArguments = "Looks like you provided too much arguments for your command:";
        internal static string WrongTypes = "The provided types did not match the method parameters!";
        internal static string TooManyMatches = "We've found too many matches!";
    }
}