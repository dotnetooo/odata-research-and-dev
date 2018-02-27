using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
namespace OdataTests.helpers
{
    public sealed  class SimpleLexer
    {
        string Text;
        int TextLen;
        int TextPos = 0;
        char? Ch;
        string Token;
        public SimpleLexer(string filter)
        {
            this.Text = filter;
            this.TextLen = filter.Length;
            this.TextPos = 0;
        }
        
        internal  string NextToken()
        {
            if (this.TextLen == this.TextPos) return null;
            int tokenPos = this.TextPos;
            ParseWhiteSpace();
            ReadToken();
            this.Token = this.Text.Substring(tokenPos, this.TextPos - tokenPos);
            return this.Token;
        }
        public IEnumerable<string>GetColumnToken()
        {
            while (null!=(this.Token = this.NextToken()))
            {
                if (this.Token.Contains('='))
                {
                    yield return this.Token.Split(new char[] { '=' })?.FirstOrDefault();
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
            while(!IsWhiteSpace && this.Ch.HasValue)
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
    }
    [TestClass]
    public class SimpleLexerTest
    {
        [TestMethod]
        public void runToken()
        {
            SimpleLexer lexer = new SimpleLexer("column1=value1 AND column2=value2");
            string token;
            while ((token = lexer.NextToken()) != null)
            {
                Console.WriteLine(token);
            }
            
        }
        [TestMethod]
        public void runAllTokens()
        {
            SimpleLexer lexer = new SimpleLexer("column1=value1 AND column2=value2 or column3='ruslan'");
            lexer.GetColumnToken().ToList().ForEach((c) =>
            {
                Console.WriteLine(c);
            }
            );
        }
    }
}
