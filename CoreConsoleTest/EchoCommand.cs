using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace CommandRunner.CoreConsoleTest
{
    public class EchoCommand
    {
        public Injectable Injectable { get; set; }

        [Command("echo", "Echo back anything following the command.")]
        public void Execute(List<string> args)
        {
            foreach (var arg in args) Console.WriteLine(arg);
        }

        [Command("techo", "Echo back with parsed param")]
        public void Execute(string whatever, bool say, int count, Guid itemId)
        {
            for (int i = 0; i < count; i++)
            {
                if (say)
                {
                    Console.WriteLine(whatever);
                }
            }
        }

    }
}
