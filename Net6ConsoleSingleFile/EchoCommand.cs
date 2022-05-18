﻿using CommandRunner;

namespace Net6ConsoleSingleFile
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
