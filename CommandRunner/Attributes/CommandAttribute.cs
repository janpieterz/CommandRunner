using System;

namespace CommandRunner
{
    /// <summary>
    /// Attribute to signal command method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Identifier used to select command
        /// </summary>
        public string Identifier { get; }
        /// <summary>
        /// Help printed with command
        /// </summary>
        public string Help { get;  }
        /// <summary>
        /// When executed in a menu and the command completes succesfully the up command will automatically be executed
        /// </summary>
        public bool MoveUpAfterSuccessfulExecution { get; }

        /// <summary>
        /// Creates an instance of the CommandAttribute
        /// </summary>
        public CommandAttribute(string identifier, string help = null, bool moveUpAfterSuccessfulExecution = false)
        {
            Identifier = identifier;
            Help = help;
            MoveUpAfterSuccessfulExecution = moveUpAfterSuccessfulExecution;
        }
    }
}