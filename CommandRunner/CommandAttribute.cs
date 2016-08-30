using System;

namespace CommandRunner
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class CommandAttribute : Attribute
    {
        public string Identifier { get; }
        public string Help { get;  }

        public CommandAttribute(string identifier, string help = null)
        {
            Identifier = identifier;
            Help = help;
        }
    }
}