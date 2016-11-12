using System;

namespace CommandRunner
{
    /// <summary>
    /// Attribute to signal an announcement method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NavigatableCommandAnnouncementAttribute : Attribute
    {
    }
}