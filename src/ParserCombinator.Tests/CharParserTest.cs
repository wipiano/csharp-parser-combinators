using System;
using Xunit;
using Xunit.Abstractions;
using static Xunit.Assert;
using static Xunit.AssertEx;

using static ParserCombinator.CharParsers;

namespace ParserCombinator.Tests
{
    public class CharParserTest
    {
        [Fact]
        public void AnyTest()
        {
            // Any はつねに成功
            var result = Any(Source.Create("a"));    // { IsSuccess: true, Result: 'a' }
            result.IsSuccess.IsTrue();
            result.Result.Is('a');
        }

        [Fact]
        public void DigitTest()
        {
            // 数字だったら成功
            var success = Digit(Source.Create("12a"));    // { IsSuccess: true, Result: '1' }
            success.IsSuccess.IsTrue();
            success.Result.Is('1');

            // 数字でなければ失敗
            var failed = Digit(Source.Create("a12"));     // { IsSuccess: false, Result: Exception }
            failed.IsSuccess.IsFalse();
            Throws(typeof(Exception), AccessToFailedResult(failed));
        }

        [Fact]
        public void LiteralTest()
        {
            var parser = Literal('a');
            var success = parser(Source.Create("abc"));    // { IsSuccess: true, Result: 'a' }
            success.IsSuccess.IsTrue();
            success.Result.Is('a');
            var failed = parser(Source.Create("ccc"));     // { IsSuccess: false, Result: Exception }
            failed.IsSuccess.IsFalse();
            Throws(typeof(Exception), AccessToFailedResult(failed));
        }


        [Fact]
        public void IsTest()
        {
            var lowerParser = Is(char.IsLower);    // 小文字だけ受け付けるパーサ
            var success = lowerParser(Source.Create("abc"));    // { IsSuccess: true, Result: 'a' }
            var failed = lowerParser(Source.Create("ABC"));     // { IsSuccess: false, Result: Exception }
            
            success.IsSuccess.IsTrue();
            success.Result.Is('a');

            failed.IsSuccess.IsFalse();
            Throws(typeof(Exception), AccessToFailedResult(failed));
        }

        private static Action AccessToFailedResult<T>(ParseResult<T> result) => () =>
        {
            var tmp = result.Result;
        };
    }
}