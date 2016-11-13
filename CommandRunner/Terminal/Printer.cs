using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner.Terminal
{
    internal static class Printer
    {
         internal static void PrintMenu(TerminalState state)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (state.ParentHierarchy.Any())
            {
                var currentMenuItem = state.ParentHierarchy.Last();
                if (currentMenuItem.Value.Command.AnnounceMethod != null)
                {
                    var instance = state.StatefullCommandActivator(currentMenuItem.Key);
                    currentMenuItem.Value.Command.AnnounceMethod.Invoke(instance, new object[0]);
                }
                else
                {
                    Console.WriteLine($"{state.ParentHierarchy.Last().Value.Command.Identifier} menu:");
                }
            }
            else
            {
                Console.WriteLine("Main menu:");
            }

            Console.ForegroundColor = state.TerminalColor;

            if (state.NavigatableMenu.Any())
            {
                PrintNavigatableItems(state.NavigatableMenu);
            }

            if (state.SingleCommands.Any())
            {
                PrintSingleCommands(state.SingleCommands);
            }

            if (state.ParentHierarchy.Any())
            {
                Console.WriteLine("To go to the previous menu type `up`");
            }
        }

        internal static void PrintNavigatableItems(List<NavigatableCommand> commands)
        {
            Console.WriteLine("Sub-menu's available (type help x to print sub items):");
            foreach (ICommand command in commands)
            {
                Console.Write("  ");
                command.WriteToConsole();
            }
            Console.WriteLine();
        }

        internal static void PrintSingleCommands(List<SingleCommand> commands)
        {
            Console.WriteLine("Commands: ");
            foreach (SingleCommand command in commands)
            {
                PrintCommand(command);
            }
        }

        private static void PrintCommand(SingleCommand command)
        {
            Console.Write("  ");
            command.WriteToConsole();
        }

        internal static void PrintLine()
        {
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("-");
            }
        }
    }
}
