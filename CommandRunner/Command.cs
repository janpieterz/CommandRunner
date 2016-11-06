using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    public class SingleCommand : CommandBase, ICommand
    {
        public void WriteToConsole()
        {
            Console.Write(Identifier);
            var previousColor = Console.ForegroundColor;
            bool previousParameter = false;
            
            foreach (ParameterInfo parameterInfo in Parameters)
            {
                parameterInfo.WriteToColoredConsole(previousParameter);
                previousParameter = true;
            }
            if (Parameters.Any())
            {
                Console.Write(")");
            }

            Console.ForegroundColor = previousColor;
            if (!string.IsNullOrEmpty(Help))
            {
                Console.Write($" - {Help}");
            }
            Console.Write(Environment.NewLine);
        }       
    }
}