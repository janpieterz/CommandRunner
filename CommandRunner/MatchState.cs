namespace CommandRunner
{
    internal enum MatchState
    {
        Matched,
        MissingParameter,
        TooManyArguments,
        Miss,
        WrongTypes
    }
}