using static ParserCombinator.ParseResultHelper;

namespace ParserCombinator
{
    public static class StringParsers
    {
        public static Parser<string> Literal(string literal) => source =>
        {
            var (str, next) = source.ReadString(literal.Length);
            return str == literal
                ? Success(next, literal)
                : Failed<string>(next, $"{str} is not equals {literal}");
        };
    }
}