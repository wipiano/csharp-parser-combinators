using System;
using System.Linq;
using Xunit;
using static Xunit.Assert;
using static ParserCombinator.ParseResultHelper;
using static ParserCombinator.CharParsers;

namespace ParserCombinator.Tests
{
    public class PostalCodeParserTests
    {
        [Fact]
        public void SimplePostalCodeParserTest()
        {
            // xxx-yyyy の xxx 部分
            Parser<int> leftPart = Digit.Repeat(3).Map(chars => int.Parse(new string(chars.ToArray())));

            // xxx-yyyy の yyyy 部分
            Parser<int> rightPart = Digit.Repeat(4).Map(chars => int.Parse(new string(chars.ToArray())));
            
            // xxx-yyyy の形式の郵便番号のパーサ
            Parser<PostalCode> postalCodeParser = leftPart
                .Left(Literal('-'))
                .Sequence(rightPart, (left, right) => new PostalCode(left, right));

            ParseResult<PostalCode> result = postalCodeParser(Source.Create("123-4567"));
            PostalCode postalCode = result.Result;
            
            result.IsSuccess.IsTrue();
            postalCode.IsStructuralEqual(new PostalCode(123, 4567));
        }

        [Fact]
        public void PostalCodeParserTest()
        {
            // xxx-yyyy の xxx 部分
            Parser<int> leftPart = Digit.Repeat(3).Map(chars => int.Parse(new string(chars.ToArray())));
            
            // xxx-yyyy の yyyy 部分
            Parser<int> rightPart = Digit.Repeat(4).Map(chars => int.Parse(new string(chars.ToArray())));
            
            // 普通の xxx-yyyy
            Parser<PostalCode> normal = leftPart.Left(Literal('-')).Sequence(rightPart, (l, r) => new PostalCode(l, r));
            
            // xxxyyyy
            Parser<PostalCode> withoutSeparator = leftPart.Sequence(rightPart, (l, r) => new PostalCode(l, r));
            
            Parser<PostalCode> postalCode = normal.Or(withoutSeparator);
            
            // 〒 が付加されてもよい
            Parser<PostalCode> postalCodeParser = Literal('〒').Right(postalCode).Or(postalCode);
            
            var expected = new PostalCode(123, 4567);
            postalCodeParser(Source.Create("123-4567")).Result.IsStructuralEqual(expected);
            postalCodeParser(Source.Create("1234567")).Result.IsStructuralEqual(expected);
            postalCodeParser(Source.Create("〒123-4567")).Result.IsStructuralEqual(expected);
            postalCodeParser(Source.Create("〒1234567")).Result.IsStructuralEqual(expected);
        }
    }

    public class PostalCode
    {
        public int LeftPart { get; }
        
        public int RightPart { get; }

        public PostalCode(int left, int right)
        {
            this.LeftPart = left;
            this.RightPart = right;
        }

        public override string ToString() => $"{LeftPart}-{RightPart}";
    }
}