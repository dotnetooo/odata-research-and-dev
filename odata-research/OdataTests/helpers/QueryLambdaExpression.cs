using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace OdataTests.helpers
{
   public class QueryLambdaExpression<TModel>: QueryNodeVisitor<Expression>

    {
        ParameterExpression userParam = Expression.Parameter(typeof(TModel), "Model");// lambda expression parameter
        public QueryLambdaExpression()
        {

        }
        public ParameterExpression LambdaParam
        {
            get { return userParam; }
        }
        public override Expression Visit(BinaryOperatorNode nodeIn)
        {
            BinaryExpression binaryExpression = Expression.MakeBinary(nodeIn.OperatorKind.ToExpressionKind(),
                nodeIn.Left.Accept<Expression>(this)
               , nodeIn.Right.Accept<Expression>(this));
            return binaryExpression;
        }

        public override Expression Visit(ConvertNode nodeIn)
        {
            var value = (nodeIn.Source as SingleValuePropertyAccessNode)?.Property?.Name;
            MemberExpression memberExpression = Expression.Property(userParam, value);
            return memberExpression;
        }
        public override Expression Visit(ConstantNode nodeIn)
        {
            return Expression.Constant(nodeIn.Value, typeof(string));
        }
    }
}
