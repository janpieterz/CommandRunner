using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner
{
    public class ReflectedMethodCommand : ICommand
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

        void ICommand.Execute(List<string> args)
        {
            var @class = Activator.CreateInstance(_methodInfo.DeclaringType);
            _methodInfo.Invoke(@class, new object[] {args});
        }
    }
}