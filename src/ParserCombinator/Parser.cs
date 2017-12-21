namespace ParserCombinator
{
    public delegate ParseResult<T> Parser<T>(Source source);

    public static class ParserExtensions
    {
        public static bool TryParse<T>(this Parser<T> parser, Source source, out ParseResult<T> result)
        {
            result = parser(source);
            return result.IsSuccess;
        }
    }
}