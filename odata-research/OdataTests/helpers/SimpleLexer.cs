using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
namespace OdataTests.helpers
{
    internal class Token
    {
        internal Token()
        {

        }
        internal enum TokenType
        {
            UNDEFINED=0,
            COMPARISON=1,
            LOGICAL=2,
            LITERAL=3
        }
           
    }
    public sealed  class SimpleLexer
    {
        string Text;
        int TextLen;
        int TextPos = 0;
        char? Ch;
        string Token;
        TokenType SqlTokenType = TokenType.LEFTOPER;
        private static char[] Operators = new char[] { '=', '>', '<', '!' };// to be growing
        private static string[] ComparisonOperators = new string[] {"=","!=",">=","<=",">","<" };
        private static string[] LogicalOperators = new string[] { "AND", "OR", "NOT" };//to be growing
        public SimpleLexer(string filter)
        {
            this.Text = filter;
            this.TextLen = filter.Length;
            this.TextPos = 0;
            this.Ch = this.Text[this.TextPos];
        }
        
        internal  string NextToken()
        {
            if (this.TextLen == this.TextPos) return null;
            int tokenPos = this.TextPos;
            SqlTokenType = (SqlTokenType == TokenType.COMPARISON) ? TokenType.RIGHTOPER :
                          TokenType.LEFTOPER;
            switch(this.Ch)
            {
                case '=':
                    SqlTokenType = TokenType.COMPARISON;
                    ReadOperator();
                    break;
                case '!':
                    SqlTokenType = TokenType.COMPARISON;
                    ReadOperator();
                    break;
                case '>':
                    SqlTokenType = TokenType.COMPARISON;
                    ReadOperator();
                    break;
                case '<':
                    SqlTokenType = TokenType.COMPARISON;
                    ReadOperator();
                    break;
                default:
                    ParseWhiteSpace();
                    ReadToken();
                    break;
            }
            this.Token = this.Text.Substring(tokenPos, this.TextPos - tokenPos);
            if (LogicalOperators.FirstOrDefault((c)=>string.Equals(c,this.Token?.Trim(),
                                 StringComparison.OrdinalIgnoreCase))!=null)
            {
                SqlTokenType = TokenType.LOGICAL;
            }
            
            return this.Token;
        }
        public IEnumerable<string>GetColumnToken()
        {
            while (null!=(this.Token = this.NextToken()))
            {
                if (this.SqlTokenType == TokenType.LEFTOPER)
                {
                    yield return this.Token;
                }
                else
                {
                    continue;
                }
            }
        }
        private bool IsWhiteSpace
        {
            get
            {
                return this.Ch != null && Char.IsWhiteSpace(this.Ch.Value);
            }
        }
        private void ParseWhiteSpace()
        {
            while(IsWhiteSpace)
            {
                this.NextChar();
            }
        }
        private void ReadToken()
        {
            if (!Ch.HasValue) NextChar();
            while(this.Ch.HasValue && !IsWhiteSpace && !Operators.Contains(this.Ch.Value))
            {
                NextChar();
            }
        }
        private void ReadLiteral()
        {
            if (!Ch.HasValue) NextChar();
            while (this.Ch.HasValue && !IsWhiteSpace)
            {
                NextChar();
            }
        }
        private void ReadOperator()
        {
            while(this.Ch.HasValue && Operators.Contains(this.Ch.Value))
            {
                NextChar();
            }
        }
        private void NextChar()
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
        internal enum TokenType
        {
            UNDEFINED = 0,
            COMPARISON = 1,
            LOGICAL = 2,
            LEFTOPER = 3,
            RIGHTOPER=4,
        }
    }
    [TestClass]
    public class SimpleLexerTest
    {
        [TestMethod]
        public void runToken()
        {
            SimpleLexer lexer = new SimpleLexer("column1!=value1 AND column2=value2");
            string token;
            while ((token = lexer.NextToken()) != null)
            {
                Console.WriteLine(token);
            }
            
        }
        [TestMethod]
        public void runAllTokens()
        {
            SimpleLexer lexer = new SimpleLexer("column1=value1 AND column2=value2 or column3!='ruslan'");
            lexer.GetColumnToken().ToList().ForEach((c) =>
            {
                Console.WriteLine(c);
            }
            );
        }
    }
}
