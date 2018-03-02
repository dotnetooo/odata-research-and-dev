using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OdataTests.helpers
{
    public sealed class SimpleLexer
    {
        string Text;
        int TextLen;
        int TextPos = 0;
        char? Ch;
        Node currentNode;
        string NodeText;
        const char QUERYOPTIONIDENTIFIER = '@';
        const char DELIMITER = '&';
        const char SEPARATOR = ',';
        StringBuilder _tokenizedFilter = new StringBuilder();
        TokenType SqlTokenType = TokenType.LEFTOPER;
        private static char[] Operators = new char[] { '=', '>', '<', '!' };// to be growing
        private static string[] ComparisonOperators = new string[] { "=", "!=", ">=", "<=", ">", "<" };
        private static string[] LogicalOperators = new string[] { "AND", "OR", "NOT" };//to be growing
        public SimpleLexer(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException($"Invalid filter {filter}.");

            this.Text = filter;
            this.TextLen = filter.Length;
            this.TextPos = 0;
            this.Ch = this.Text[this.TextPos];
        }

        internal string TokenizedSql
        {
            get { return _tokenizedFilter.ToString(); }
        }
        internal Node NextNode()
        {
            if (this.TextLen == this.TextPos) return null;
            ParseWhiteSpace();
            int tokenPos = this.TextPos;
            SqlTokenType = (SqlTokenType == TokenType.COMPARISON) ? TokenType.RIGHTOPER :
                          TokenType.LEFTOPER;
            switch (this.Ch)
            {
                case SEPARATOR:
                    break;
                case QUERYOPTIONIDENTIFIER:
                    //1. parserwhitespacee 2.read token; 
                    break;
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
            this.NodeText = this.Text.Substring(tokenPos, this.TextPos - tokenPos);
            return this.currentNode;
        }
        public IEnumerable<Node> GetToken()
        {
            while (null != (this.currentNode = this.NextNode()))
            {
               yield  return this.currentNode;
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
            while (IsWhiteSpace)
            {
                this.NextChar();
            }
        }
        private void ReadToken()
        {
            if (!Ch.HasValue) NextChar();
            while (this.Ch.HasValue && !IsWhiteSpace && !Operators.Contains(this.Ch.Value))
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
            while (this.Ch.HasValue && Operators.Contains(this.Ch.Value))
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
            RIGHTOPER = 4,
            END = 5
        }

        private void WriteToken(string token)
        {
            switch (this.SqlTokenType)
            {
                case TokenType.COMPARISON:
                    _tokenizedFilter.AppendFormat("{0}", token);
                    break;
                case TokenType.LEFTOPER:
                    _tokenizedFilter.AppendFormat(" @{0}", token);
                    break;
                case TokenType.LOGICAL:
                    _tokenizedFilter.AppendFormat(" {0} ", token);
                    break;
                case TokenType.RIGHTOPER:
                    _tokenizedFilter.AppendFormat("{0} ", token);
                    break;

            }
        }
    }


    #region Tokens
    public abstract class Node : IQueryNode
    {
        protected readonly NodeType _nodeType;
        protected readonly List<Node> _innerNodes;
        protected readonly string _text;

        protected Node(string text, NodeType nodeType, params Node[] node)
        {
            _innerNodes = node.ToList();
            _text = text;
            _nodeType = nodeType;
        }
       
        public NodeType RocketNodeType
        {
            get { return _nodeType; }
        }
        public enum NodeType
        {
            UNDEFINED=0,
            SELECT=1,
            FROM=2,
            JOIN=3,
            WHERE=4,
            ALIAS=5,
            ORDERBY=6,
            SELECTSTATEMENT=7
        }
        public abstract void Accept<T>(IRocketVisitor<T> visitor) where T : class;
        public virtual List<Node> Nodes => _innerNodes?.OrderBy(n => n._nodeType).ToList();
        public string Text => _text;
    }
    public interface IRocketVisitor<T> where T:class
    {
        T Visit(Node token);
    }
    public interface IQueryNode
    {
        void Accept<T>(IRocketVisitor<T> visitor) where T : class;
        List<Node> Nodes { get; }
        string Text { get; }
    }
    public sealed class WhereNode:Node
    {
        public WhereNode(string text, params Node[] nodes) : base(text, NodeType.WHERE, nodes) { }
        
        public override void Accept<T>(IRocketVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
    public sealed class SelectStatementNode:Node
    {
        public SelectStatementNode(string text, params Node[] nodes) : base(text, NodeType.SELECTSTATEMENT, nodes) { }

        public override void Accept<T>(IRocketVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
        public override List<Node> Nodes
        {
            get { return base.Nodes; }
        }
      
    }
        
#endregion
    [TestClass]
    public class SimpleLexerTest
    {
   
      
    }
}
