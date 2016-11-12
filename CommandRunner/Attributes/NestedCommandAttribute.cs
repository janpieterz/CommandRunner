using System;

namespace CommandRunner
{
    /// <summary>
    /// Attribute signals a Nested command
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NestedCommandAttribute : Attribute
    {
        /// <summary>
        /// Identifier used to prepend child commands
        /// </summary>
        public string Identifier { get; }
        /// <summary>
        /// Created a NestedCommandAttribute
        /// </summary>
        public NestedCommandAttribute(string identifier)
        {
            Identifier = identifier;
        }
    }
}