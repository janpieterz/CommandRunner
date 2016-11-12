using System.Collections.Generic;
namespace CommandRunner
{
    internal class InputParser
    {
        public static IEnumerable<string> ParseInputToArguments(string input)
        {
            List<string> arguments = new List<string>();
            string buffer = string.Empty;
            bool enclosedProcessing = false;
            foreach (char c in input)
            {
                if (c == ' ' && !enclosedProcessing)
                {
                    arguments.Add(buffer);
                    buffer = string.Empty;
                }
                else if (c == '"')
                {
                    enclosedProcessing = !enclosedProcessing;
                }
                else
                {
                    buffer = buffer + c;
                }
            }
            if (!string.IsNullOrWhiteSpace(buffer))
            {
                arguments.Add(buffer.ToLowerInvariant());
            }
            return arguments;
        }
    }
}