using System.Collections.Generic;
using System.Linq;

namespace CommandRunner.Terminal {
    internal sealed class TerminalState : State {

        internal void SetMenu(NavigatableCommand parentCommand, object instance)
        {
            ActiveMenu = parentCommand.SubItems;
            ParentHierarchy[parentCommand.Type] = new ParentCommand()
            {
                Command = parentCommand,
                Instance = instance
            };
        }

        internal void MoveUp()
        {
            if (!ParentHierarchy.Any()) return;
            var previous = ParentHierarchy.Last();
            ParentHierarchy.Remove(previous.Key);
            if (ParentHierarchy.Any())
            {
                ActiveMenu = ParentHierarchy.Last().Value.Command.SubItems;
            }
            else
            {
                ActiveMenu = FullMenu;

            }
        }
    }
}