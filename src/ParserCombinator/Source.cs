using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserCombinator
{
    /// <summary>
    /// Parser への入力
    /// </summary>
    public struct Source
    {
        // 最初に与えられた文字列をもっておく
        private readonly string _source;
        
        // 現在位置
        private readonly int _pos;

        private Source(string source, int pos)
        {
            _source = source;
            _pos = pos;
        }
    
        /// <summary>文字列の先頭をさす Source を作成します</summary>
        public static Source Create(string source)
            => new Source(source, 0);
    
        /// <summary>一文字読み出します</summary>
        public (char c, Source source) Read()
        {
            if (_source.Length <= _pos)
            {
                // source の終わりを超えて読もうとした場合は Exception
                throw new EndOfSourceException(this);
            }
    
            return (_source[_pos], new Source(_source, _pos + 1));
        }
    
        /// <summary>指定した文字数ぶん char を読み出します</summary>
        public (IEnumerable<char> chars, Source source) ReadChars(int count)
        {
            if (_source.Length < _pos + count)
            {
                // 読み出そうとしている長さが source をこえていたら Exception
                throw new EndOfSourceException(this);
            }
    
            return (_source.Skip(_pos).Take(count), new Source(_source, _pos + count));
        }
    
        /// <summary>指定した長さの文字列を読み出します</summary>
        public (string s, Source source) ReadString(int length)
        {
            if (_source.Length < _pos + length)
            {
                // 読み出そうとしている長さが source をこえていたら Exception
                throw new EndOfSourceException(this);
            }
    
            return (_source.Substring(_pos, length), new Source(_source, _pos + length));
        }
    
        /// <summary>Source の終わりに達したときの Exception</summary>
        public class EndOfSourceException : Exception
        {
            private static readonly string EndOfSource = "EndOfSource";
            
            /// <summary>例外発生時の Source</summary>
            public Source Source { get; }
    
            internal EndOfSourceException(Source source)
                : base(EndOfSource)
            {
                this.Source = source;
            }
        }
    }
}