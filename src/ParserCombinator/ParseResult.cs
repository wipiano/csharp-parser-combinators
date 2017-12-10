using System;

namespace ParserCombinator
{
    public struct ParseResult<T>
    {
        /// <summary>実行後の Source</summary>
        public Source Source { get; }
        
        /// <summary>成功したかどうか</summary>
        public bool IsSuccess { get; }

        /// <summary>パース結果</summary>
        public T Result =>
            this.IsSuccess ? _result : throw new Exception($"Parse error: {Reason}");

        private readonly T _result;
        
        // 失敗した理由
        public string Reason { get; }
        
        internal ParseResult(Source source, bool isSuccess, T result, string reason)
        {
            this.Source = source;
            this.IsSuccess = isSuccess;
            _result = result;
            this.Reason = reason;
        }
    }
}