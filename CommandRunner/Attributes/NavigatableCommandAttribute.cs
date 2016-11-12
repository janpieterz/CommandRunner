using System;

namespace CommandRunner
{
    /// <summary>
    /// Attribute to signal a navigatable command attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class NavigatableCommandAttribute : Attribute
    {
        /// <summary>
        /// Identifier used to select command
        /// </summary>
        public string Identifier { get; }
        /// <summary>
        /// Help printed with command
        /// </summary>
        public string Help { get; }
        /// <summary>
        /// Creates an instance of the NavigatableCommandAttribute
        /// </summary>
        public NavigatableCommandAttribute(string identifier, string help = null)
        {
            Identifier = identifier;
            Help = help;
        }
    }
}