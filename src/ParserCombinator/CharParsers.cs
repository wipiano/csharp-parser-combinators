using System;

namespace ParserCombinator
{
    using static ParseResultHelper;
    
    public static class CharParsers
    {
        public static Parser<char> Any { get; } = (Source s) =>
        {
            var (c, next) = s.Read();
            return Success(next, c);
        };

        public static Parser<char> Digit { get; } = (Source s) =>
        {
            var (c, next) = s.Read();
            return char.IsDigit(c) ? Success(next, c) : Failed<char>(next, "Is not a digit.");
        };
        
        public static Parser<char> Literal(char literal) => (Source s) =>
        {
            var (c, next) = s.Read();
            return c == literal ? Success(next, c) : Failed<char>(next, $"{c} is not equals {literal}");
        };
        
        public static Parser<char> Is(Func<char, bool> predicate) => (Source s) =>
        {
            var (c, next) = s.Read();
            return predicate(c) ? Success(next, c) : Failed<char>(next, $"predicate({c}) returns false.");
        };
    }
}