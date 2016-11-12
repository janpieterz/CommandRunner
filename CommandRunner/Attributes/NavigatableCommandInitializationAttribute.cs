using System;

namespace CommandRunner
{
    /// <summary>
    /// Attribute signals a NavigatableCommand Initialization method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NavigatableCommandInitializationAttribute : Attribute
    {
    }
}