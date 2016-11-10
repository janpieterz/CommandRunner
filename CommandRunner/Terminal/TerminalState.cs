using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner.Terminal {
    internal sealed class TerminalState : State {

        public void SetMenu(List<ICommand> newMenu, ICommand parentCommand = null)
        {
            Menu = newMenu;
            var command = parentCommand as NavigatableCommand;
            if (command != null && ParentHierarchy.ContainsKey(parentCommand.Type))
            {
                ParentHierarchy[parentCommand.Type].Command = command;
            }
        }

        public void MoveUp()
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