using Xunit;
using static ParserCombinator.StringParsers;

namespace ParserCombinator.Tests
{
    public class StringTest
    {
        [Fact]
        public void StringSupportTest()
        {
            var sampleParser = Literal("parser").Left(Literal(" ")).Sequence(Literal("combinator")).AsString();

            var result1 = sampleParser("parser combinator");
            result1.IsSuccess.IsTrue();
            result1.Result.Is("parsercombinator");

            var result2 = sampleParser("paaaaaaaaaaaaaaaa");
            result2.IsSuccess.IsFalse();
        }
    }
}