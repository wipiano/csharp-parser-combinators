namespace ParserCombinator
{
    public delegate ParseResult<T> Parser<T>(Source source);
}