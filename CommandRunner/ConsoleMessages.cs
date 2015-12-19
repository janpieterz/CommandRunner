using System;

namespace CommandRunner
{
    /// <summary>
    /// Helper to print messages to the console in a uniform way for Help and Error messages.
    /// </summary>
    public class ConsoleMessages
    {
        /// <summary>
        /// Write a help message to the console.
        /// </summary>
        /// <param name="messages">Messages you'd like to write.</param>
        public static void Help(params string[] messages)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
            Console.ResetColor();
        }
        /// <summary>
        /// Write an error message to the console.
        /// </summary>
        /// <param name="messages">Messages you'd like to write.</param>
        public static void Error(params string[] messages)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
            Console.ResetColor();

        }
        /// <summary>
        /// Write an exception to the console including stacktrace.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">An option, extra message.</param>
        public static void Error(Exception exception, string message = null)
        {
            Error($"{message} Exception: {exception.Message} {Environment.NewLine} {exception.StackTrace}");
        }
    }
}