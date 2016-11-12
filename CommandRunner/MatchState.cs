namespace CommandRunner
{
    internal enum MatchState
    {
        Matched,
        MissingParameter,
        TooManyParameters,
        Miss,
        WrongTypes
    }
}