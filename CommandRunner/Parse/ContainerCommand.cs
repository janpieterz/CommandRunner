namespace CommandRunner.Parse
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
    }
}