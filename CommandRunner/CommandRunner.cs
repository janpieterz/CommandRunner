using System;

namespace CommandRunner
{
    /// <summary>
    /// Simple helper class to bootstrap your Command runner.
    /// </summary>
    public static class CommandRunner
    {
        /// <summary>
        /// Bootstrap your CommandRunner
        /// </summary>
        /// <param name="commands">The commands you want to be made available.</param>
        public static void Run(params ICommand[] commands)
        {
            var runner = new Runner();
            runner.Run(commands);
            Console.ReadLine();
        }
    }
}