using System;
using System.Collections.Immutable;
using System.Linq;
using static ParserCombinator.ParseResultHelper;

namespace ParserCombinator
{
    public static class Combinators
    {
        public static Parser<ImmutableList<T>> Many<T>(this Parser<T> parser)
        {
            ParseResult<ImmutableList<T>> Impl(Source s, ImmutableList<T> results)
            {
                var result = parser(s);

                return result.IsSuccess
                    ? Impl(result.Source, results.Add(result.Result))
                    : Success(s, results);
            }

            return (Source s) => Impl(s, ImmutableList<T>.Empty);
        }

        public static Parser<ImmutableList<T>> Repeat<T>(this Parser<T> parser, int count)
        {
            ParseResult<ImmutableList<T>> Impl(Source s, int c, ImmutableList<T> results)
            {
                if (c == 0)
                {
                    // 0 回を指定されたら終わり
                    return Success(s, results);
                }

                var result = parser(s);

                return result.IsSuccess
                    ? Impl(result.Source, c - 1, results.Add(result.Result))
                    : Failed<ImmutableList<T>>(result.Source, result.Reason);
            }

            return (Source s) => Impl(s, count, ImmutableList<T>.Empty);
        }

        public static Parser<ImmutableList<T>> Sequence<T>(this Parser<T> first, Parser<T> second) =>
            first.Sequence(second, (f, s) => ImmutableList<T>.Empty.Add(f).Add(s));

        public static Parser<ImmutableList<T>> Sequence<T>(this Parser<ImmutableList<T>> first, Parser<T> second) =>
            first.Sequence(second, (f, s) => f.Add(s));

        public static Parser<TResult> Sequence<TFirst, TSecond, TResult>(this Parser<TFirst> first,
            Parser<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) =>
            (Source s) =>
            {
                var firstResult = first(s);
                if (firstResult.IsSuccess)
                {
                    var secondResult = second(firstResult.Source);

                    return secondResult.IsSuccess
                        ? Success(secondResult.Source, resultSelector(firstResult.Result, secondResult.Result))
                        : Failed<TResult>(secondResult.Source, secondResult.Reason);
                }
                else
                {
                    return Failed<TResult>(firstResult.Source, firstResult.Reason);
                }
            };

        public static Parser<T> Or<T>(this Parser<T> left, Parser<T> right) => (Source s) =>
        {
            var leftResult = left(s);

            return leftResult.IsSuccess
                ? leftResult
                : right(s);
        };

        public static Parser<TLeft> Left<TLeft, TRight>(this Parser<TLeft> left, Parser<TRight> right) =>
            left.Sequence(right, (l, r) => l);

        public static Parser<TRight> Right<TLeft, TRight>(this Parser<TLeft> left, Parser<TRight> right) =>
            left.Sequence(right, (l, r) => r);

        public static Parser<TResult>
            Map<TParser, TResult>(this Parser<TParser> parser, Func<TParser, TResult> mapper) =>
            (Source s) =>
            {
                var result = parser(s);
                return result.IsSuccess
                    ? Success(result.Source, mapper(result.Result))
                    : Failed<TResult>(result.Source, result.Reason);
            };

        public static Parser<string> AsString(this Parser<ImmutableList<string>> parser) =>
            parser.Map(strings => string.Join(string.Empty, strings));

        public static Parser<string> AsString(this Parser<ImmutableList<char>> parser) =>
            parser.Map(chars => new string(chars.ToArray()));

        public static Parser<ImmutableList<T>> AtLeastOne<T>(this Parser<T> parser)
        {
            ParseResult<ImmutableList<T>> Impl(Source source, ImmutableList<T> results, bool isFirst = false)
            {
                var result = parser(source);
                return result.IsSuccess
                    ? Impl(result.Source, results.Add(result.Result))
                    : isFirst
                        ? Failed<ImmutableList<T>>(result.Source, result.Reason)
                        : Success(source, results);
            }

            return (Source s) => Impl(s, ImmutableList<T>.Empty, true);
        }
    }
}