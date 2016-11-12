using System.Collections.Generic;
using System.Linq;

namespace CommandRunner.Terminal {
    internal sealed class TerminalState : State {

        internal void SetMenu(List<ICommand> newMenu, ICommand parentCommand = null)
        {
            Menu = newMenu;
            var command = parentCommand as NavigatableCommand;
            if (command != null && ParentHierarchy.ContainsKey(parentCommand.Type))
            {
                ParentHierarchy[parentCommand.Type].Command = command;
            }
        }

        internal void MoveUp()
        {
            if (!ParentHierarchy.Any()) return;
            var previous = ParentHierarchy.Last();
            ParentHierarchy.Remove(previous.Key);
            if (ParentHierarchy.Any())
            {
                Menu = ParentHierarchy.Last().Value.Command.SubItems;
            }
            else
            {
                Menu = Menu;

            }
        }
    }
}