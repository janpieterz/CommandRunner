namespace CommandRunner.CommandLine {
    internal sealed class CommandLineState : State
    {
        internal bool InHelpMode => Arguments[0] == "-help" || Arguments[0] == "/?" || Arguments[0] == "-h";
    }
}