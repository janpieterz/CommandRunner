using System.Collections.Generic;

namespace CommandRunner
{
    internal class ContainerCommand : IMenuItem
    {
        public ContainerCommand(string identifier, string help)
        {
            Title = identifier;
            Help = help;
        }

        public string Title { get; }
        public string Help { get; }
        public List<IMenuItem> Items { get; set; } = new List<IMenuItem>();
    }
}