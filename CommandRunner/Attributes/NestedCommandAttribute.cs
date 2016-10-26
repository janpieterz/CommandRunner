using System;

namespace CommandRunner
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NestedCommandAttribute : CommandAttribute
    {
        public NestedCommandAttribute(string identifier, string help = null) : base(identifier, help)
        {
        }
    }
}