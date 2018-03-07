using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace OdataTests.helpers
{
    [TestClass]
    public class baseLexerTest
    {
        [DataTestMethod]
        [DataRow("count(1), average (2)")]
        //[DataRow("count  (1),average(2)")]
        //[DataRow("count(  1), average(2)")]
        //[DataRow("count(1), average(2)")]
        public void shouldSuccessInMetric(string pattern)
        {
            QueryLexer lexer = new QueryLexer(pattern);
            string token = null;
            while(!string.IsNullOrEmpty(token=lexer.NextToken()))
            {
                Console.WriteLine(token);
            }
             
        }
    }

    

    public abstract class BaseLexer<T> : ILexer<T> where T:class
    {
        protected readonly string Text;
        protected readonly int TextLen;
        protected int TextPos;
        protected char? Ch;
        protected string TokenValue;
        protected BaseLexer(string querySringValue)
        {
            this.Text = querySringValue ?? throw new ArgumentNullException(nameof(querySringValue));
            this.TextLen = this.Text.Length;
            this.TextPos = 0;
            this.Ch = this.Text[this.TextPos];

        }
        public abstract string NextToken();
        protected void SetTokenValue(int tokenPos)
        {
            this.TokenValue = this.Text.Substring(tokenPos, this.TextPos - tokenPos)?.Trim();
        }
        protected virtual void ReadToken(Func<char,bool> functNextChar)
        {
            if (!Ch.HasValue) return;
            while (this.Ch.HasValue && functNextChar.Invoke(this.Ch.Value))
            {
                NextChar();
            }
        }
        protected void ParseWhiteSpace()
        {
            while (IsWhiteSpace)
            {
                this.NextChar();
            }
        }
        protected bool IsWhiteSpace
        {
            get
            {
                return this.Ch != null && Char.IsWhiteSpace(this.Ch.Value);
            }
        }
        protected virtual void NextChar()
        {
            if (this.TextPos < this.TextLen)
            {
                this.TextPos++;
                if (this.TextPos < this.TextLen)
                {
                    this.Ch = this.Text[this.TextPos];
                    return;
                }

            }
            this.Ch = null;
        }
        public abstract T GetNode();
        
    }
    public static class QueryConstants
    {
        public const char SymbComma = ',';
        public const char SymbAt = '@';
        public const char SymbEqual = '=';
        public const char SymbQueryConcatenate = '&';
        public const char SymdDot = '.';
        public const char SymbSingleQuote = '\'';
        public const char SymbForwardSlash = '/';
        public const char SymbBackSlash = '\\';
        public const char SymbOpenParen = '(';
        public const char SymbClosedParen = ')';
        public const string KeywordAverage = "average";
        public const string KeywordCount = "count";
    }
    public class QueryLexer :BaseLexer<QueryNode>
    {
        public QueryLexer(string queryOptionValue):base(queryOptionValue)
        { }
        public override QueryNode GetNode()
        {
            throw new NotImplementedException();
        }
        public override string NextToken()
        {
            if (this.TextLen == this.TextPos) return null;
            ParseWhiteSpace();
            int tokenPos = this.TextPos;
            switch(this.Ch)
            {
                case QueryConstants.SymbComma:
                    ReadToken((c) => !c.Equals(QueryConstants.SymbOpenParen));
                    break;
                case QueryConstants.SymbOpenParen:
                    NextChar();
                    tokenPos = this.TextPos;
                    ReadToken((c) => !c.Equals(QueryConstants.SymbClosedParen));
                    break;
                case QueryConstants.SymbClosedParen:
                    while(IsWhiteSpace || !Char.Equals(this.Ch,QueryConstants.SymbComma) && this.Ch.HasValue)
                    {
                        NextChar();
                    }
                    NextChar();
                    tokenPos = this.TextPos;
                    ReadToken((c) => !c.Equals(QueryConstants.SymbOpenParen));
                    break;
                default:
                    ReadToken((c) => !c.Equals(QueryConstants.SymbComma) && 
                                     ! IsWhiteSpace &&
                                     !c.Equals(QueryConstants.SymbOpenParen) &&
                                     !c.Equals(QueryConstants.SymbClosedParen));
                    break;
            }
            SetTokenValue(tokenPos);
            return this.TokenValue;
        }
        
    }
    public abstract class QueryNode
    {
        protected readonly string _value;
        protected readonly Type _valueType;
        protected readonly QueryNodeType _nodeType;
        protected QueryNode(string value,Type valueType,QueryNodeType nodeType)
        {
            this._value = value ?? throw new ArgumentNullException(nameof(value));
            this._valueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            this._nodeType = nodeType;

        }
        public string Value => this._value;

        public Type ValueType => this._valueType;

        public enum QueryNodeType
        {
            UNDEFINED=0,
            METRIC=1,
            FILTER=2
        }
    }
    public sealed class FilterNode:QueryNode
    {
        public FilterNode(string value, Type valueType) :base(value,valueType,QueryNodeType.FILTER)
        {
           
        }
    }

}
