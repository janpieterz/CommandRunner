﻿using System;

namespace CommandRunner.CoreConsole
{
    [NestedCommand("nest")]
    public class NestingCommand
    {
        [Command("hello", "Say hello")]
        public void Hello()
        {
            Console.WriteLine("Hello");
        }

        [Command("hi", "Say hi")]
        public void SayHi()
        {
            Console.WriteLine("Hi");
        }
    }
}