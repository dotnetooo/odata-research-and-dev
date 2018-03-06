using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
namespace OdataTests.helpers
{
    [TestClass]
    public class EpresionLexerTests
    {
      
        [DataTestMethod]
        [DataRow("count(1),average(2)")]
        public void testMetric(string metric)
        {
            ExpressionLexer lexer = new ExpressionLexer(metric);
            ExpressionToken token;
            List<string> tokens = new List<string>();
            while ((token = lexer.NextToken()).Kind != ExpressionTokenKind.End)
            {
                tokens.Add(token.Text);
            }
            Assert.IsTrue(tokens.Count == 8);
        }
        [TestMethod]
        public void testFilter()
        {
            string metric = "column1 eq 1 AND column2 eq 2 or column3 eq'ruslan'";
            ExpressionLexer lexer = new ExpressionLexer(metric);
            ExpressionToken token;
            List<ExpressionToken> tokens = new List<ExpressionToken>();
            while ((token = lexer.NextToken()).Kind != ExpressionTokenKind.End)
            {
                tokens.Add(token);
            }
            Assert.IsTrue(tokens.Count == 13);
        }
        [TestMethod]
        public void testFilterProcced()
        {
            string metric = "column1!=value1 AND column2=value2 or column3='ruslan'";
            ExpressionLexer lexer = new ExpressionLexer(metric);
            ExpressionToken token;
            List<ExpressionToken> tokens = new List<ExpressionToken>();
            while ((token = lexer.NextToken()).Kind != ExpressionTokenKind.End)
            {
                tokens.Add(token);
            }
            Assert.IsTrue(tokens.Count == 13);
        }
        [DataTestMethod]
        [DataRow("count(1,8), average(2)", DisplayName = "MetricWithSpace")]
        [DataRow("count(1),average(2)", DisplayName = "MetricNoSpace")]
        [DataRow("count(1  ),average(2)", DisplayName = "MetricSpaceInFuction")]
        [DataRow("count(1  ) average(2)", DisplayName = "MetricNoDelimiter")]
        public void parseMetricSuccess(string metric)
        {
            RocketUriParser parser = new RocketUriParser();
            var result = parser.ParseMetric(metric).ToList();
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0].Function == "count");
            Assert.IsTrue(result[1].Function == "average");
        }

    }
    public class RocketUriParser
    {
        public RocketUriParser() { }
        public IEnumerable<FunctionToken> ParseMetric(string metric)
        {
            ExpressionLexer lexer = new ExpressionLexer(metric);
            List<FunctionToken> tokens = new List<FunctionToken>();
            ExpressionToken token;
            while ((token = lexer.NextToken()).Kind != ExpressionTokenKind.End)
            {
                if (token.Kind == ExpressionTokenKind.Comma)
                {
                    token = lexer.NextToken();
                }
                if(token.Kind!=ExpressionTokenKind.Identifier)
                {
                    throw new Exception($"Can't process {metric} at position: {token.Position} token: {token.Text}");
                }
                FunctionToken metricToken = new FunctionToken();
                metricToken.Text = token.Text;
                metricToken.Function = token.Text;
                token = lexer.NextToken();
                if (token.Kind == ExpressionTokenKind.OpenParen)
                {
                    token = lexer.NextToken();
                }
                if (token.Kind == ExpressionTokenKind.Identifier)
                {
                    metricToken.Arguments.Add(token.Text);
                    token = lexer.NextToken();
                }
                else
                {
                    throw new Exception($"Can't process {metric} at position: {token.Position} token: {token.Text}");
                }
                while (token.Kind == ExpressionTokenKind.Comma)
                {
                    token = lexer.NextToken();
                    if (token.Kind == ExpressionTokenKind.Identifier)
                    {
                        metricToken.Arguments.Add(token.Text);
                    }
                    else
                    {
                        throw new Exception($"Can't process {metric} at position: {token.Position} token: {token.Text}");
                    }
                    token = lexer.NextToken();
                }
                if (token.Kind!= ExpressionTokenKind.CloseParen)
                {
                    throw new Exception($"Can't process {metric} at position: {token.Position} token: {token.Text}");
                }
                
                tokens.Add(metricToken);

            }
            return tokens;
        }
    }

    public class ExpressionLexer
    {
        string Text;
        int TextLen;
        int TextPos = 0;
        char? Ch;
        ExpressionToken Token;
        string TokenText;
        public ExpressionLexer(string text)
        {
            this.Text = text;
            this.TextLen = text.Length;
            this.TextPos = 0;
            this.Ch = this.Text[this.TextPos];
        }

        public ExpressionToken NextToken()
        {
            if (this.TextLen == this.TextPos)
            {
                Token = new ExpressionToken() { Kind = ExpressionTokenKind.End };
                return Token;
            }
            ParseWhiteSpace();
            int tokenPos = this.TextPos;
            ExpressionTokenKind tokenKind;
            switch (this.Ch)
            {
                case '=':
                    tokenKind = ExpressionTokenKind.EqualSign;
                    NextChar();
                    break;

                case '<':
                    tokenKind = ExpressionTokenKind.LessSign;
                    if (TryPeekNext().IsComparisonOperator)
                    {
                        NextChar();
                        tokenPos = this.TextPos;
                        tokenKind = ExpressionTokenKind.LessOrEqualSign;
                    }
                    else
                    {
                        NextChar();
                    }
                    break;

                case '>':
                    tokenKind = ExpressionTokenKind.GreaterSign;
                    if (TryPeekNext().IsComparisonOperator)
                    {
                        NextChar();
                        tokenPos = this.TextPos;
                        tokenKind = ExpressionTokenKind.GreaterOrEqualSign;
                    }
                    else
                    {
                        NextChar();
                    }
                    break;

                case '(':
                    tokenKind = ExpressionTokenKind.OpenParen;
                    NextChar();
                    break;
                case ')':
                    tokenKind = ExpressionTokenKind.CloseParen;
                    NextChar();
                    break;
                case ',':
                    tokenKind = ExpressionTokenKind.Comma;
                    NextChar();
                    break;
                case '.':
                    tokenKind = ExpressionTokenKind.Dot;
                    NextChar();
                    break;
                case '!':
                    tokenKind = ExpressionTokenKind.Exclamation;
                    if ((TryPeekNext().Kind == ExpressionTokenKind.EqualSign))
                    {
                        ReadToken(c => !Char.IsLetterOrDigit(c));
                        tokenKind = ExpressionTokenKind.NotEqualSign;
                    }
                    else
                    {
                        NextChar();
                    }
                    break;
                case '-':
                    tokenKind = ExpressionTokenKind.Minus;
                    NextChar();
                    break;
                case '\'':
                    tokenKind = ExpressionTokenKind.SingleQuote;
                    NextChar();
                    break;
                default:
                    ReadToken(c => c != ',' && c != ')' && c != '(' && c != '\'' && c!='=' && c!='!');
                    tokenKind = ExpressionTokenKind.Identifier;
                    break;
            }
            SetText(tokenPos);
            Token = new ExpressionToken() { Kind = tokenKind };
            Token.Text = this.TokenText;
            Token.Position = tokenPos;
            return Token;
        }

        public ExpressionToken TryPeekNext()
        {
            int savedPosition = this.TextPos;
            Char? c = this.Ch;
            string text = this.TokenText;
            ExpressionToken saved = Token;
            NextChar();
            ExpressionToken result = NextToken();
            this.TextPos = savedPosition;
            this.Ch = c;
            Token = saved;
            this.TokenText = text;
            return result;
        }
        protected void SetText(int tokenPos)
        {
            this.TokenText = this.Text.Substring(tokenPos, this.TextPos - tokenPos);
        }
        protected bool IsWhiteSpace
        {
            get
            {
                return this.Ch != null && Char.IsWhiteSpace(this.Ch.Value);
            }
        }
        protected void ParseWhiteSpace()
        {
            while (IsWhiteSpace)
            {
                this.NextChar();
            }
        }
        protected void ReadToken(Func<char, bool> boudary = null)
        {
            while (this.Ch.HasValue && !IsWhiteSpace && (boudary?.Invoke(this.Ch.Value) ?? true))
            {
                NextChar();
            }
        }
        protected void NextChar()
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
    }

    [DebuggerDisplay("{Kind} @ {Position}: [{Text}]")]
    public struct ExpressionToken
    {
        /// <summary>Token representing gt keyword</summary>
        internal static readonly ExpressionToken GreaterThan = new ExpressionToken { Text = ExpressionConstants.KeywordGreaterThan, Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Token representing eq keyword</summary>
        internal static readonly ExpressionToken EqualsTo = new ExpressionToken { Text = ExpressionConstants.KeywordEqual, Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Token representing lt keyword</summary>
        internal static readonly ExpressionToken LessThan = new ExpressionToken { Text = ExpressionConstants.KeywordLessThan, Kind = ExpressionTokenKind.Identifier, Position = 0 };

        /// <summary>Kind of token.</summary>
        internal ExpressionTokenKind Kind;

        /// <summary>Token text.</summary>
        internal string Text;

        /// <summary>Position of token.</summary>
        internal int Position;

        /// <summary>Checks whether this token is a comparison operator.</summary>
        internal bool IsComparisonOperator
        {
            get
            {
                if (this.Kind != ExpressionTokenKind.Identifier)
                {
                    return false;
                }

                return
                    this.Text == ExpressionConstants.KeywordEqual ||
                    this.Text == ExpressionConstants.KeywordNotEqual ||
                    this.Text == ExpressionConstants.KeywordLessThan ||
                    this.Text == ExpressionConstants.KeywordGreaterThan ||
                    this.Text == ExpressionConstants.KeywordLessThanOrEqual ||
                    this.Text == ExpressionConstants.KeywordGreaterThanOrEqual ||
                    this.Kind == ExpressionTokenKind.EqualSign ||
                    this.Kind == ExpressionTokenKind.GreaterSign ||
                    this.Kind == ExpressionTokenKind.LessOrEqualSign ||
                    this.Kind == ExpressionTokenKind.GreaterOrEqualSign;
            }
        }

        /// <summary>Checks whether this token is an equality operator.</summary>
        internal bool IsEqualityOperator
        {
            get
            {
                return
                    this.Kind == ExpressionTokenKind.Identifier &&
                    (this.Text == ExpressionConstants.KeywordEqual ||
                     this.Text == ExpressionConstants.KeywordNotEqual);
            }
        }

        /// <summary>Checks whether this token is a valid token for a literal.</summary>
        internal bool IsLiteral
        {
            get
            {
                return
                    this.Kind == ExpressionTokenKind.NullLiteral ||
                    this.Kind == ExpressionTokenKind.BinaryLiteral ||
                    this.Kind == ExpressionTokenKind.BooleanLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeLiteral ||
                    this.Kind == ExpressionTokenKind.GuidLiteral ||
                    this.Kind == ExpressionTokenKind.DateTimeOffsetLiteral ||
                    this.Kind == ExpressionTokenKind.DurationLiteral ||
                    this.Kind == ExpressionTokenKind.StringLiteral ||
                    IsNumeric(this.Kind);
            }
        }
        internal static bool IsNumeric(ExpressionTokenKind id)
        {
            return
                id == ExpressionTokenKind.IntegerLiteral || id == ExpressionTokenKind.DecimalLiteral ||
                id == ExpressionTokenKind.DoubleLiteral || id == ExpressionTokenKind.Int64Literal ||
                id == ExpressionTokenKind.SingleLiteral;
        }
        /// <summary>Provides a string representation of this token.</summary>
        /// <returns>String representation of this token.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} @ {1}: [{2}]", this.Kind, this.Position, this.Text);
        }

        /// <summary>Gets the current identifier text.</summary>
        /// <returns>The current identifier text.</returns>
        internal string GetIdentifier()
        {
            return this.Text;
        }

        /// <summary>Checks that this token has the specified identifier.</summary>
        /// <param name="id">Identifier to check.</param>
        /// <returns>true if this is an identifier with the specified text.</returns>
        internal bool IdentifierIs(string id)
        {
            return this.Kind == ExpressionTokenKind.Identifier && this.Text == id;
        }


    }

    public enum ExpressionTokenKind
    {
        /// <summary>Unknown.</summary>
        Unknown,

        /// <summary>End of text.</summary>
        End,

        /// <summary>'=' - equality character.</summary>
        Equal,

        /// <summary>Identifier.</summary>
        Identifier,

        /// <summary>NullLiteral.</summary>
        NullLiteral,

        /// <summary>BooleanLiteral.</summary>
        BooleanLiteral,

        /// <summary>StringLiteral.</summary>
        StringLiteral,

        /// <summary>IntegerLiteral.</summary>
        IntegerLiteral,

        /// <summary>Int64 literal.</summary>
        Int64Literal,

        /// <summary>Single literal.</summary>
        SingleLiteral,

        /// <summary>DateTime literal.</summary>
        DateTimeLiteral,

        /// <summary>Decimal literal.</summary>
        DecimalLiteral,

        /// <summary>Double literal.</summary>
        DoubleLiteral,

        /// <summary>GUID literal.</summary>
        GuidLiteral,

        /// <summary>Binary literal.</summary>
        BinaryLiteral,

        /// <summary>DateTimeOffset literal.</summary>
        DateTimeOffsetLiteral,

        /// <summary>Duration literal.</summary>
        DurationLiteral,

        /// <summary>Exclamation.</summary>
        Exclamation,

        /// <summary>OpenParen.</summary>
        OpenParen,

        /// <summary>CloseParen.</summary>
        CloseParen,

        /// <summary>Comma.</summary>
        Comma,

        /// <summary>Minus.</summary>
        Minus,

        /// <summary>Slash.</summary>
        Slash,

        /// <summary>Question.</summary>
        Question,

        /// <summary>Dot.</summary>
        Dot,

        /// <summary>Star.</summary>
        Star,

        /// <summary>Colon.</summary>
        Colon,

        /// <summary>Semicolon</summary>
        Semicolon,

        /// <summary>Spatial Literal</summary>
        GeographylLiteral,

        /// <summary>Geometry Literal</summary>
        GeometryLiteral,

        /// <summary>Whitespace</summary>
        WhiteSpace,

        SingleQuote,

        EqualSign,

        GreaterSign,

        LessSign,

        GreaterOrEqualSign,

        LessOrEqualSign,

        NotEqualSign

    }

    public static class ExpressionConstants
    {
        /// <summary>"add" keyword for expressions.</summary>
        internal const string KeywordAdd = "add";

        /// <summary>"and" keyword for expressions.</summary>
        internal const string KeywordAnd = "and";

        /// <summary>"asc" keyword for expressions.</summary>
        internal const string KeywordAscending = "asc";

        /// <summary>"desc" keyword for expressions.</summary>
        internal const string KeywordDescending = "desc";

        /// <summary>"div" keyword for expressions.</summary>
        internal const string KeywordDivide = "div";

        /// <summary>"eq" keyword for expressions.</summary>
        internal const string KeywordEqual = "eq";

        /// <summary>"false" keyword for expressions.</summary>
        internal const string KeywordFalse = "false";

        /// <summary>"gt" keyword for expressions.</summary>
        internal const string KeywordGreaterThan = "gt";

        /// <summary>"ge" keyword for expressions.</summary>
        internal const string KeywordGreaterThanOrEqual = "ge";

        /// <summary>"lt" keyword for expressions.</summary>
        internal const string KeywordLessThan = "lt";

        /// <summary>"le" keyword for expressions.</summary>
        internal const string KeywordLessThanOrEqual = "le";

        /// <summary>"mod" keyword for expressions.</summary>
        internal const string KeywordModulo = "mod";

        /// <summary>"mul" keyword for expressions.</summary>
        internal const string KeywordMultiply = "mul";

        /// <summary>"not" keyword for expressions.</summary>
        internal const string KeywordNot = "not";

        /// <summary>"ne" keyword for expressions.</summary>
        internal const string KeywordNotEqual = "ne";

        /// <summary>"null" keyword for expressions.</summary>
        internal const string KeywordNull = "null";

        /// <summary>"or" keyword for expressions.</summary>
        internal const string KeywordOr = "or";

        /// <summary>"sub" keyword for expressions.</summary>
        internal const string KeywordSub = "sub";

        /// <summary>"true" keyword for expressions.</summary>
        internal const string KeywordTrue = "true";

        /// <summary>'INF' literal, as used in XML for infinity.</summary>
        internal const string InfinityLiteral = "INF";

        /// <summary>'NaN' literal, as used in XML for not-a-number values.</summary>
        internal const string NaNLiteral = "NaN";
    }

    public abstract class Token
    {
        public string Text { get; set; }
    }
    public class FunctionToken : Token
    {
        public List<object> _functionArguments = new List<object>();
        public FunctionToken()
        {

        }
        public string Function { get; set; }
        public List<object> Arguments
        {
            get { return _functionArguments; }
        }
    }
}
