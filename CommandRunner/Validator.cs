using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandRunner
{
    internal static class Validator
    {
        internal static void ValidateEnumerable(List<ICommand> commands)
        {
            List<string> wrongEnumerableCommands = FindWrongEnumerables(commands);
            if (wrongEnumerableCommands.Any())
            {
                throw new Exception(
                    string.Format(
                        "We've found occurances of an enumerable being used at a wrong position. Enumerables can only be used as the last parameter. Commands: [{0}]",
                        string.Join(",", wrongEnumerableCommands)));
            }
        }

        private static List<string> FindWrongEnumerables(List<ICommand> commands)
        {
            List<string> items = new List<string>();
            var wrongEnumerables =
                commands.Where(
                    x =>
                        x.Parameters.Take(x.Parameters.Count - 1)
                            .Any(parameter => parameter.ParameterType.IsEnumerable()));
            items.AddRange(wrongEnumerables.Select(x => $"{x.Type.Name}:{x.Identifier}"));

            foreach (NavigatableCommand navigatableCommand in commands.OfType<NavigatableCommand>())
            {
                items.AddRange(FindWrongEnumerables(navigatableCommand.SubItems));
            }
            return items;
        }

        internal static void ValidateHelp(List<ICommand> commands)
        {
            List<string> helpCommands = FindHelpCommands(commands);
            if (helpCommands.Any())
            {
                throw new Exception(string.Format("You have commands that start the prohibited `help` keywords (`help`, `/?`, `-h`). Commands: [{0}]", string.Join(",", helpCommands)));
            }
        }

        internal static List<string> FindHelpCommands(List<ICommand> commands)
        {
            List<string> items = new List<string>();
            var firstLevelHelps =
                commands.Where(
                    x => x.Identifier.StartsWith("help") || x.Identifier.StartsWith("/?") || x.Identifier.StartsWith("-h"));
            items.AddRange(firstLevelHelps.Select(x => $"{x.Type.Name}:{x.Identifier}"));

            foreach (NavigatableCommand navigatableCommand in commands.OfType<NavigatableCommand>())
            {
                items.AddRange(FindHelpCommands(navigatableCommand.SubItems));
            }
            return items;
        }

        internal static void ValidateDuplicates(List<ICommand> commands)
        {
            List<string> dupes = FindDuplicates(commands);
            if (dupes.Any())
            {
                throw new Exception(string.Format("You have duplicate commands. Commands: [{0}]", string.Join(",", dupes)));
            }
        }

        internal static List<string> FindDuplicates(List<ICommand> commands)
        {
            List<string> items = new List<string>();
            var firstLevelDuplicates = commands.GroupBy(x => x.Identifier).Where(x => x.Count() > 1).ToList();
            foreach (IGrouping<string, ICommand> grouping in firstLevelDuplicates)
            {
                foreach (ICommand command in grouping)
                {
                    items.Add($"{command.Type.Name}:{command.Identifier}");
                }
            }

            foreach (NavigatableCommand navigatableCommand in commands.OfType<NavigatableCommand>())
            {
                items.AddRange(FindDuplicates(navigatableCommand.SubItems));
            }
            return items;
        }
    }
}
