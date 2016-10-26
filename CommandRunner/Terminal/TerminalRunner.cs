using System;
namespace CommandRunner.Terminal
{
    public class TerminalRunner : IStartableRunner {
        public TerminalRunner (RunnerConfiguration configuration)
        {
            Console.Title = configuration.Title;
        }
        public void Start() {

        }
                // private void RunTerminalMode()
        // {
        //     var menuItemList = _settings.Menu;
        //     if (!menuItemList.Any())
        //     {
        //         WriteErrorMessage("Please add commands to add functionality.");
        //         return;
        //     }
        //     string input;
        //     do
        //     {
        //         Console.WriteLine($"Available commands:");

        //         menuItemList = menuItemList.OrderBy(x => x.Title).ToList();
        //         menuItemList.ForEach(OutputMenuItem);

        //         Console.Write($"{Environment.NewLine}Command> ");
        //         input = Console.ReadLine() ?? string.Empty;
        //         SetupConsoleErrorColor();
        //         // var commandWithArgs = InputParser.FindCommand(menuItemList, InputParser.ParseInputToArguments(input));
        //         SetupConsoleRunnerColor();
                // if (commandWithArgs.Item1 != null)
                // {
                //     try
                //     {
                //         SetupConsoleCommandColor();
                //         commandWithArgs.Item1.Execute(commandWithArgs.Item2.ToList());
                //     }
                //     catch (Exception exception)
                //     {
                //         WriteErrorMessage(exception.Message);
                //     }
                //     finally
                //     {
                //         SetupConsoleRunnerColor();
                //     }
                //     Console.WriteLine();
                // }
        //     } while (string.IsNullOrEmpty(input) || !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase));
        // }

        // private void OutputMenuItem(IMenuItem menuItem)
        // {
        //     if (menuItem is ContainerCommand)
        //     {
        //         Console.WriteLine();
        //         Console.WriteLine($"{menuItem.Title.ToLowerInvariant()} {menuItem.Help}");
        //         (menuItem as ContainerCommand).Items.ForEach(OutputMenuItem);
        //         Console.WriteLine();
        //     }
        //     else
        //     {
        //         Console.WriteLine($"  {menuItem.Title.ToLowerInvariant()}: {menuItem.Help}");
        //     }
        // }

        // private void WriteErrorMessage(string error)
        // {
        //     SetupConsoleErrorColor();
        //     Console.WriteLine(error);
        //     SetupConsoleRunnerColor();
        // }
    }
}