using System;

namespace CommandRunner
{
    /// <summary>
    /// Attribute signals a Nested command
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NestedCommandAttribute : CommandAttribute
    {
        /// <summary>
        /// Created a NestedCommandAttribute
        /// </summary>
        public NestedCommandAttribute(string identifier, string help = null) : base(identifier, help)
        {
        }
    }
}