using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using static ParserCombinator.CharParsers;
using static ParserCombinator.StringParsers;
using static ParserCombinator.Combinators;
using ParserCombinator;

namespace ParserCombinator.Tests
{
    /// <summary>
    /// json parser
    /// see: https://www.json.org
    /// </summary>
    public static class JsonParser
    {
        private static readonly Parser<MetaNumber> Number = CreateNumber();

        private static Parser<MetaNumber> CreateNumber()
        {
            var digits = Digit.AtLeastOne();
            
            var e = "E+".Or("e+").Value(Sign.Positive)
                .Or("E-".Or("e-").Value(Sign.Negative))
                .Or('E'.Or('e').Value(Sign.Positive));
            
            var exp = e.Sequence(digits.AsString(), (sign, d) => new MetaExp(sign, d));

            var frac = '.'.Right(digits).AsString();

            var integer = Digit.Sequence(Is(c => '1' <= c && c <= '9')).Sequence(digits).AsMetaInt(Sign.Positive)
                .Or('-'.Right(Digit).Sequence(Is(c => '1' <= c && c <= '9')).Sequence(digits).AsMetaInt(Sign.Negative))
                .Or('-'.Right(Digit).AsMetaInt(Sign.Negative))
                .Or(Digit.AsMetaInt(Sign.Positive));

            var number = integer.Sequence(frac, (i, f) => (integer: i, frac: f)).Sequence(exp,
                    (num, ex) => new MetaNumber(num.integer, num.frac, ex))
                .Or(integer.Sequence(exp, (i, ex) => new MetaNumber(i, null, ex)))
                .Or(integer.Sequence(frac, (i, fr) => new MetaNumber(i, fr)))
                .Or(integer.Map(i => new MetaNumber(i, null)));

            return number;
        }
        
        private static Parser<MetaString> StringLiteral => CreateStringLiteral();

        private static Parser<MetaString> CreateStringLiteral()
        {
            var hex = Is(c => char.IsDigit(c) || ('a' <= c && c <= 'f') || ('A' <= c && c <= 'F'));

            var jsonChar = Is(c => !char.IsControl(c) && c != '\\' && c != '"')
                .Or(Literal("\\\"").Value('"'))
                .Or(Literal("\\\\").Value('\\'))
                .Or(Literal("\\b").Value('\b'))
                .Or(Literal("\\f").Value('\f'))
                .Or(Literal("\\n").Value('\n'))
                .Or(Literal("\\r").Value('\r'))
                .Or(Literal("\\t").Value('\t'))
                .Or(Literal("\\u").Sequence(hex.Repeat(4), (s, code) => Convert.ToChar(Convert.ToInt32(new string(code.ToArray())))));

            var chars = jsonChar.AtLeastOne();

            var stringLiteral = Literal("\"\"").Value(string.Empty)
                .Or('"'.Right(chars).Left('"').AsString())
                .Map(s => new MetaString(s));

            return stringLiteral;
        }
        
        private static readonly Parser<ImmutableList<char>> OptionalSpace = ' '.Many();

        private static Parser<IJsonValue> Value => StringLiteral.AsJsonValue()
            .Or(Number.AsJsonValue())
            .Or(Lazy(() => Object).AsJsonValue())
            .Or(Lazy(() => Array).AsJsonValue())
            .Or(Literal("true").Value(MetaBool.True).AsJsonValue())
            .Or(Literal("false").Value(MetaBool.False).AsJsonValue())
            .Or(Literal("null").Value(MetaNull.Instance).AsJsonValue());

        private static Parser<MetaArray> Array =>
            '['.Left(OptionalSpace).Left(']').Value(new MetaArray(Enumerable.Empty<IJsonValue>()))
                .Or('['.Left(OptionalSpace).Right(Lazy(() => Value)).Left(OptionalSpace.Right(',')).AtLeastOne().Left(OptionalSpace.Right(']')).Map(values => new MetaArray(values)));

        private static Parser<MetaJsonObject> Object =>
            '{'.Left(OptionalSpace).Left('}').Value(new MetaJsonObject(new Dictionary<string, IJsonValue>(0)))
            .Or('{'.Left(OptionalSpace).Right(StringLiteral.Left(OptionalSpace.Sequence(':').Sequence(OptionalSpace))).Left(OptionalSpace.Sequence('}')))
        
        private static Parser<MetaInt> AsMetaInt(this Parser<ImmutableList<char>> parser, Sign sign)
            => parser.AsString().Map(s => new MetaInt(sign, s));

        private static Parser<MetaInt> AsMetaInt(this Parser<char> parser, Sign sign)
            => parser.Map(c => new MetaInt(sign, c.ToString()));

        private static Parser<IJsonValue> AsJsonValue<T>(this Parser<T> parser) where T : IJsonValue
            => parser.Map(obj => obj as IJsonValue);
    }

    public interface IJsonValue
    {
        
    }
    
    public class MetaJsonObject : IJsonValue
    {
        public Dictionary<string, IJsonValue> Members { get; };

        public MetaJsonObject(Dictionary<string, IJsonValue> members)
        {
            this.Members = members;
        }
    }


    public class MetaArray : IJsonValue
    {
        public IReadOnlyList<IJsonValue> Values { get; }

        public MetaArray(IEnumerable<IJsonValue> values)
        {
            this.Values = values.ToList();
        }
    }

    public class MetaBool : IJsonValue
    {
        public static readonly MetaBool True = new MetaBool(true);
        public static readonly MetaBool False = new MetaBool(false);
        
        public bool Value { get; }

        private MetaBool(bool value)
        {
            this.Value = value;
        }
    }

    public class MetaNull : IJsonValue
    {
        public static readonly MetaNull Instance = new MetaNull();
        private MetaNull() {}
    }

    public class MetaString : IJsonValue
    {
        public string Value { get; }

        public MetaString(string value)
        {
            this.Value = value;
        }
    }
    
    public class MetaNumber : IJsonValue
    {
        public MetaInt Int { get; }
        public string Frac { get; }
        public MetaExp? Exp { get; }

        public MetaNumber(MetaInt metaInt, string frac, MetaExp? exp = null)
        {
            this.Int = metaInt;
            this.Frac = frac;
            this.Exp = exp;
        }
    }

    public struct MetaInt
    {
        public Sign Sign { get; }
        public string Digits { get; }

        public MetaInt(Sign sign, string digits)
        {
            this.Sign = sign;
            this.Digits = digits;
        }
    }
    
    public struct MetaExp
    {
        public Sign Sign { get; }
        public string Digits { get; }
        
        public MetaExp(Sign sign, string digits)
        {
            this.Sign = sign;
            this.Digits = digits;
        }
    }
    
    public enum Sign : byte
    {
        Positive,
        Negative
    }
}