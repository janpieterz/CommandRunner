using System;
using System.Collections.Generic;

namespace CommandRunner.ClassicConsole
{
    public class EchoCommand
    {

        [Command("echo", "Echo back anything following the command.")]
        public void Execute(List<string> args)
        {
            foreach (var arg in args) Console.WriteLine(arg);
        }
    }
}
