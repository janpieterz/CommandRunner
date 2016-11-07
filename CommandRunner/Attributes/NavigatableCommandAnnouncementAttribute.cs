using System;

namespace CommandRunner
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NavigatableCommandAnnouncementAttribute : Attribute
    {
    }
}