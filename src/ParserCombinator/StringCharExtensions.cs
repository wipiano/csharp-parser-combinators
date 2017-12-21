using System.Collections.Immutable;
using System.Text;

namespace ParserCombinator
{
    using static CharParsers;
    using static StringParsers;
    
    public static class StringCharExtensions
    {
        public static Parser<ImmutableList<string>> Many(this string str)
            => Literal(str).Many();

        public static Parser<ImmutableList<char>> Many(this char c)
            => Literal(c).Many();

        public static Parser<ImmutableList<string>> Repeat(this string str, int count)
            => Literal(str).Repeat(count);

        public static Parser<ImmutableList<char>> Repeat(this char c, int count)
            => Literal(c).Repeat(count);

        public static Parser<ImmutableList<string>> Sequence(this string first, string second)
            => Literal(first).Sequence(Literal(second));

        public static Parser<ImmutableList<string>> Sequence(this string first, Parser<string> second)
            => Literal(first).Sequence(second);

        public static Parser<ImmutableList<string>> Sequence(this string first, Parser<ImmutableList<string>> second)
            => Literal(first).Sequence(second);

        public static Parser<ImmutableList<string>> Sequence(this Parser<string> first, string second)
            => first.Sequence(Literal(second));

        public static Parser<ImmutableList<string>> Sequence(this Parser<ImmutableList<string>> first, string second)
            => first.Sequence(Literal(second));
        
        public static Parser<ImmutableList<char>> Sequence(this char first, char second)
            => Literal(first).Sequence(Literal(second));

        public static Parser<ImmutableList<char>> Sequence(this char first, Parser<char> second)
            => Literal(first).Sequence(second);

        public static Parser<ImmutableList<char>> Sequence(this char first, Parser<ImmutableList<char>> second)
            => Literal(first).Sequence(second);

        public static Parser<ImmutableList<char>> Sequence(this Parser<char> first, char second)
            => first.Sequence(Literal(second));

        public static Parser<ImmutableList<char>> Sequence(this Parser<ImmutableList<char>> first, char second)
            => first.Sequence(Literal(second));

        public static Parser<string> Or(this string left, string right)
            => Literal(left).Or(Literal(right));
        
        public static Parser<string> Or(this string left, Parser<string> right)
            => Literal(left).Or(right);

        public static Parser<string> Or(this Parser<string> left, string right)
            => left.Or(Literal(right));

        public static Parser<char> Or(this char left, char right)
            => Literal(left).Or(Literal(right));
        
        public static Parser<char> Or(this char left, Parser<char> right)
            => Literal(left).Or(right);

        public static Parser<char> Or(this Parser<char> left, char right)
            => left.Or(Literal(right));

        public static Parser<string> Left(this string left, string right)
            => Literal(left).Left(Literal(right));

        public static Parser<string> Left<T>(this string left, Parser<T> right)
            => Literal(left).Left(right);

        public static Parser<T> Left<T>(this Parser<T> left, string right)
            => left.Left(Literal(right));
        
        public static Parser<char> Left(this char left, char right)
            => Literal(left).Left(Literal(right));

        public static Parser<char> Left<T>(this char left, Parser<T> right)
            => Literal(left).Left(right);

        public static Parser<T> Left<T>(this Parser<T> left, char right)
            => left.Left(Literal(right));

        public static Parser<string> Right(this string left, string right)
            => Literal(left).Right(Literal(right));

        public static Parser<T> Right<T>(this string left, Parser<T> right)
            => Literal(left).Right(right);

        public static Parser<string> Right<T>(this Parser<T> left, string right)
            => left.Right(Literal(right));
        
        public static Parser<char> Right(this char left, char right)
            => Literal(left).Right(Literal(right));

        public static Parser<T> Right<T>(this char left, Parser<T> right)
            => Literal(left).Right(right);

        public static Parser<char> Right<T>(this Parser<T> left, char right)
            => left.Right(Literal(right));

        public static Parser<ImmutableList<string>> AtLeastOne(string str) => Literal(str).AtLeastOne();

        public static Parser<ImmutableList<char>> AtLeastOne(char c) => Literal(c).AtLeastOne();
        
        public static Parser<string> AsString(this Parser<ImmutableList<string>> parser) =>
            parser.Map(strings => string.Join(string.Empty, strings));

        public static Parser<string> AsString(this Parser<ImmutableList<char>> parser) =>
            parser.Map(chars =>
            {
                var sb = new StringBuilder();
                foreach (var c in chars)
                {
                    sb.Append(c);
                }

                return sb.ToString();
            });
    }
}