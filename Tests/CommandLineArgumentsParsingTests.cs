using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CommandRunner.Tests
{
    
    public class CommandLineArgumentsParsingTests
    {
        public static IEnumerable<object[]> GetArguments()
        {
            yield return new object[] { "hello simple test", new List<string>() {"hello", "simple", "test"} };
            yield return new object[] { "hello \"test with parenthesis\"", new List<string>() { "hello", "test with parenthesis"} };
            yield return new object[] { "hello \"test with parenthesis\" and more", new List<string>() { "hello", "test with parenthesis", "and", "more"} };
            yield return new object[] { "hello \"test with parenthesis\" and more \"and other parenthesis\"", new List<string>() { "hello", "test with parenthesis", "and", "more" , "and other parenthesis" } };
        }

        [Theory]
        [MemberData(nameof(GetArguments))]
        public void ParseInputString(string input, List<string> output)
        {
            Console.WriteLine(Assembly.GetEntryAssembly().Location);
            var arguments = InputParser.ParseInputToArguments(input).ToList();
            for (int i = 0; i < arguments.Count(); i++)
            {
                Assert.Equal(output[i], arguments[i]);
            }
        }
    }
}

