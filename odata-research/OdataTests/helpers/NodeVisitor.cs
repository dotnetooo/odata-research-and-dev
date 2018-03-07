using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.OData.UriParser.Aggregation;

namespace OdataTests.helpers
{
    public class NodeVisitor : ISyntacticTreeVisitor<string>
    {
        public string Visit(AllToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(AnyToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(BinaryOperatorToken tokenIn)
        {
            return $"{tokenIn.Left.Accept<string>(this)} {tokenIn.OperatorKind.ToSqlOperator()} { tokenIn.Right.Accept<string>(this)}";
        }

        public string Visit(DottedIdentifierToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(ExpandToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(ExpandTermToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(FunctionCallToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(LambdaToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(LiteralToken tokenIn)
        {
            return tokenIn.Value.GetType().Equals(typeof(string)) ? $"'{tokenIn.Value.ToString()}'":
                                           $"{tokenIn.Value}";
        }

        public string Visit(InnerPathToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(OrderByToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(EndPathToken tokenIn)
        {
            return tokenIn.Identifier;
        }

        public string Visit(CustomQueryOptionToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(RangeVariableToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(SelectToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(StarToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(UnaryOperatorToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(FunctionParameterToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(AggregateToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(AggregateExpressionToken tokenIn)
        {
            throw new NotImplementedException();
        }

        public string Visit(GroupByToken tokenIn)
        {
            throw new NotImplementedException();
        }
    }
}
