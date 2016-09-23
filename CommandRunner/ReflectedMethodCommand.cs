using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public class ReflectedMethodCommand : TypedParameterExecution, ICommand
    {
        private readonly MethodInfo _methodInfo;
        public string Title { get; private set; }
        public string Help { get; private set; }
        public ReflectedMethodCommand(string title, string help, MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            Title = title;
            Help = help;
        }

        void ICommand.Execute(List<string> arguments)
        {
            var @class = Activator.CreateInstance(_methodInfo.DeclaringType);
            Execute(@class, _methodInfo, arguments);
        }
    }
}