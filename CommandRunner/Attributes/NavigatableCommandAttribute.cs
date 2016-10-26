using System;

namespace CommandRunner
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NavigatableCommandAttribute : Attribute
    {
        public string Identifier { get; }
        public string Help { get;  }

        public NavigatableCommandAttribute(string identifier, string help = null)
        {
            Identifier = identifier;
            Help = help;
        }
    }
}