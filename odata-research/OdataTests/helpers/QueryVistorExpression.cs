using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
namespace OdataTests.helpers
{
   public  class QueryVistorExpression: QueryNodeVisitor<Expression>
    {
        public override Expression Visit(BinaryOperatorNode nodeIn)
        {
            BinaryExpression binaryExpression = Expression.MakeBinary(nodeIn.OperatorKind.ToExpressionKind(),
                nodeIn.Left.Accept<Expression>(this)
               , nodeIn.Right.Accept<Expression>(this));
            return binaryExpression;
        }
        public override Expression Visit(SingleValuePropertyAccessNode nodeIn)
        {
            return null;
        }
        public override Expression Visit(ConvertNode nodeIn)
        {
          var value= (nodeIn.Source as SingleValuePropertyAccessNode)?.Property?.Name;
            UnaryExpression typeAsExpression = Expression.TypeAs(Expression.Constant(value, typeof(string)), typeof(string));
            return typeAsExpression;
        }
        public override Expression Visit(ConstantNode nodeIn)
        {
            return Expression.Constant(nodeIn.LiteralText, typeof(string));
        }
    }
}
