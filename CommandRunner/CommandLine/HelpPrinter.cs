using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner.CommandLine
{
    internal class HelpPrinter
    {
        private int _level;
        internal static void PrintHelp(string title, List<ICommand> menu)
        {
            Console.WriteLine("Help for command runner {0}", title);
            HelpPrinter printer = new HelpPrinter();
            printer.PrintItems(menu);
        }

        private void PrintItems(List<ICommand> menu)
        {
            if (menu.Any())
            {
                _level++;
            }
            var navigatableItems = menu.OfType<NavigatableCommand>().ToList();
            var singleCommandItems = menu.OfType<SingleCommand>().ToList();
            if (navigatableItems.Any())
            {
                Console.Write(new string(' ', _level > 1 ? _level * 2 : 0));
                Console.WriteLine("Sub-menu's available:");
                foreach (NavigatableCommand navigatableCommand in navigatableItems.OrderBy(x => x.Identifier))
                {
                    PrintNavigatableItem(navigatableCommand);
                }
            }

            if (singleCommandItems.Any())
            {
                Console.Write(new string(' ', _level > 1 ? _level * 2 : 0));
                Console.WriteLine("Commands: ");
                foreach (SingleCommand singleCommand in singleCommandItems.OrderBy(x => x.Identifier))
                {
                    PrintSingleCommand(singleCommand);
                }
            }
            if (_level > 1)
            {
                _level--;
            }
        }

        private void PrintNavigatableItem(NavigatableCommand command)
        {
            Console.Write(new string(' ', _level * 2));
            command.WriteToConsole();
            PrintItems(command.SubItems);
            Console.WriteLine();
        }

        private void PrintSingleCommand(SingleCommand command)
        {
            Console.Write(new string(' ', _level * 2));
            command.WriteToConsole();
        }
    }
}
