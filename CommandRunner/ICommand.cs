using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner
{
    public interface ICommand
    {
        MethodInfo MethodInfo { get; }
        List<ParameterInfo> Parameters { get; }
        string Identifier { get; }
        void WriteToConsole();
        MatchState Match(List<string> arguments);
        List<string> ArgumentsWithoutIdentifier(List<string> arguments);
        Type Type { get; }

    }
}