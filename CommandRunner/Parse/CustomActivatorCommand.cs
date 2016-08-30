using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner.Parse
{
    public class CustomActivatorCommand : ICommand
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
        public void Execute(List<string> args)
        {
            object @class = _activator(_methodInfo.DeclaringType);
            if (_methodInfo.GetParameters().Length > 0)
            {
                _methodInfo.Invoke(@class, new object[] {args});
            }
            else
            {
                _methodInfo.Invoke(@class, null);
            }
            
        }
    }
}