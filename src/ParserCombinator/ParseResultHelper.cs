namespace ParserCombinator
{
    public static class ParseResultHelper
    {
        public static ParseResult<T> Success<T>(Source source, T result)
            => new ParseResult<T>(source, true, result, default);
        
        public static ParseResult<T> Failed<T>(Source source, string reason)
            => new ParseResult<T>(source, false, default, reason);
    }
}