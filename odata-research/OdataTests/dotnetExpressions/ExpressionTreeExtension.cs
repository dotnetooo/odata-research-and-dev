using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OdataTests.expressionTests
{
    [TestClass]
   public class ExpressionTreeExtension
    {
        [TestMethod]
        public void walkingTreee()
        {
            string whereClause = "colum1=value1 and column2=value3 or column3!=value3";
            // start from right
            var bexp = Expression.MakeBinary(
                ExpressionType.Or,
                Expression.MakeBinary(ExpressionType.And,
                Expression.MakeBinary(ExpressionType.Equal,
                Expression.Constant("column1"), Expression.Constant("value1")),
                Expression.MakeBinary(ExpressionType.Equal, Expression.Constant("column2"), Expression.Constant("value2")))
               , Expression.MakeBinary(ExpressionType.Equal, Expression.Constant("column3"), Expression.Constant("value3")));

            //now extending
            var expExented = Expression.MakeBinary(ExpressionType.And,
                bexp,
                Expression.MakeBinary(ExpressionType.Equal,
                Expression.Constant("column4"), Expression.Constant("value4")));

            string result = bexp.ToString();
            string extResult = expExented.ToString();

            // that will do sql
            MyExpressionVisitor visitor = new MyExpressionVisitor();
            SqlStatement sqlStatement = new SqlStatement(visitor);
            sqlStatement.Select = Expression.Constant("column1,column2,column4");
            sqlStatement.From = Expression.Constant("MyTable");

            //setting an expression
            sqlStatement.Where = bexp;
            // generate sql statement
            string myStatement = sqlStatement.Render();

            // setting a new expression
            sqlStatement.Where = expExented;
            // adding a new field to an expression
            sqlStatement.Select = Expression.Coalesce(sqlStatement.Select, Expression.Constant("column5"));
            // new statement 
            string nexSatement = sqlStatement.Render();

        }
    }

    public class MyExpressionVisitor
    {
        public MyExpressionVisitor() : base() { }
        public string Visit(Expression expression)
        {
            // quick example!
            if (expression.NodeType == ExpressionType.And || expression.NodeType == ExpressionType.Or)
            {
                return VisitBinary(expression as BinaryExpression);
            }
            else if (expression.NodeType == ExpressionType.Equal)
            {
                return VisitBinary(expression as BinaryExpression);
            }
            else if (expression.NodeType == ExpressionType.Constant)
            {
                return VisitConstant(expression as ConstantExpression);
            }
            else if(expression.NodeType==ExpressionType.Coalesce)
            {
                return VisitBinary(expression as BinaryExpression);
            }
            return "ok";
        }
        public string VisitBinary(BinaryExpression node)
        {
            return $"{this.Visit(node.Left)} { toSql(node.NodeType)} {this.Visit(node.Right)} ";
        }
        public string VisitConstant(ConstantExpression node)
        {

            return node.Value.ToString();
        }
        public string VisitParameter(ParameterExpression node)
        {

            return node.Name;
        }
       
        public static string toSql(ExpressionType expType)
        {
            if (expType == ExpressionType.And)
            {
                return "And";
            }
            else if (expType == ExpressionType.Or)
            {
                return "Or";
            }
            else if (expType == ExpressionType.NotEqual)
            {
                return "<>";
            }
            else if (expType == ExpressionType.Equal)
            {
                return "=";
            }
            else if(expType==ExpressionType.Coalesce)
            {
                return " , ";
            }
            else
            {
                return "ok";
            }
        }

    }

    public class SqlStatement
    {
        private MyExpressionVisitor Visitor;
        public SqlStatement(MyExpressionVisitor visitor) /* should be injected from di container */
        {
            Visitor = visitor;
        }
        public Expression Select { get; set; }

        public Expression From { get; set; }

        public Expression Where { get; set; }

        public string Render()
        {
            string sql = Visitor.Visit(Select);
            string from = Visitor.Visit(From);
            string where = Visitor.Visit(Where);
            return $"SELECT {sql} FROM {from} WHERE {where}";
        }
    }
}
