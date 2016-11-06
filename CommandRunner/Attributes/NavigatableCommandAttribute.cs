using System;

namespace CommandRunner
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class NavigatableCommandAttribute : Attribute
    {
        public string Identifier { get; }
        public string Help { get; }

        public NavigatableCommandAttribute(string identifier, string help = null)
        {
            Identifier = identifier;
            Help = help;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NavigatableCommandInitialisationAttribute : Attribute
    {
    }
}