using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner
{
    public class CustomActivatorCommand : TypedParameterExecution, ICommand
    {
        private readonly MethodInfo _methodInfo;
        private readonly Func<Type, object> _activator;
        public string Title { get; }
        public string Help { get; }

        public CustomActivatorCommand(string title, string help, MethodInfo methodInfo, Func<Type, object> activator)
        {
            _methodInfo = methodInfo;
            _activator = activator;
            Title = title;
            Help = help;
        }
        public void Execute(List<string> arguments)
        {
            object @class = _activator(_methodInfo.DeclaringType);
            Execute(@class, _methodInfo, arguments);
        }
    }
}