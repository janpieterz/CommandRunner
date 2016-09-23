using System.Collections.Generic;

namespace CommandRunner
{
    public interface ICommand : IMenuItem
    { 
        void Execute(List<string> arguments);

    }
}