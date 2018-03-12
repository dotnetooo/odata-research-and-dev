using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdataParserProject;
using System.Dynamic;

namespace OdataTests.dotnetExpressions
{
    [TestClass]
   public  class dotnetExpressionsDev
    {
        [TestMethod]
        public void MakeDynamicObjec()
        {
            object obj = new { name = "ruslan" };
            dynamic myobj = new ExpandoObject();
            var expandoDict = myobj as IDictionary<string, object>;
            expandoDict.Add("value1", "value1");

        }
        [TestMethod]
        public void makeOneSqlExpression()
        {

            string whereClause = "colum1=value1 and column2=value3 or column3!=value3";
            // start from right
            var bexp = Expression.MakeBinary(
                ExpressionType.Or,
                Expression.MakeBinary(ExpressionType.And,
                Expression.MakeBinary(ExpressionType.Equal,
                Expression.Constant("column1"), Expression.Constant("value1")),
                Expression.MakeBinary(ExpressionType.Equal, Expression.Constant("column2"), Expression.Constant("value2")))
               , Expression.MakeBinary(ExpressionType.NotEqual, Expression.Constant("column3"), Expression.Constant("value3")));

            //now extending
            var expExented = Expression.MakeBinary(ExpressionType.And,
                bexp,
                Expression.MakeBinary(ExpressionType.Equal,
                Expression.Constant("column4"), Expression.Constant("value4")));

            string result = bexp.ToString();
            string extResult = expExented.ToString();
            MyExpressionVisitor visitor = new MyExpressionVisitor();
            SqlStatement sqlStatement = new SqlStatement(visitor);
            // start building slq tree
            var select= Expression.Constant("Select column1,column2,column4");
            var addingFrom = Expression.Coalesce(select, Expression.Constant("From MyTable"));
            // Expression.Coalesc does not accep constants of differen type;
            string resultSql = visitor.Visit(expExented);


        }
        #region Binding Examples
        [TestMethod]
        public void bindProperty()
        {
            var par = Expression.Parameter(typeof(string), "id");
            var nObject = Expression.New(typeof(MyModel));
            var bindingExp = new[]
            {
                Expression.Bind(typeof(MyModel).GetProperty("Id"),par)
            };
            var objecInt = Expression.MemberInit(nObject, bindingExp);
            var lambda = Expression.Lambda<Func<string, MyModel>>(objecInt, par);
            var mymodel= lambda.Compile().Invoke("123");
            Assert.IsNotNull(mymodel);

        }
        #endregion
       
        [TestMethod]
        public void parseSql()
        {
            string whereClause = "colum1=value1 and column2=value3 or column3!=value3";
            // start from right
            var bexp = Expression.MakeBinary(
                ExpressionType.Or,
                Expression.MakeBinary(ExpressionType.And,
                Expression.MakeBinary(ExpressionType.Equal,
                Expression.Constant("column1"), Expression.Constant("value1")),
                Expression.MakeBinary(ExpressionType.Equal, Expression.Constant("column2"), Expression.Constant("value2")))
               ,Expression.MakeBinary(ExpressionType.Equal, Expression.Constant("column3"), Expression.Constant("value3")));

            //now extending
            var expExented = Expression.MakeBinary(ExpressionType.And,
                bexp,
                Expression.MakeBinary(ExpressionType.Equal,
                Expression.Constant("column4"), Expression.Constant("value4")));
                
            string result=bexp.ToString();
            string extResult = expExented.ToString();
            MyExpressionVisitor visitor = new MyExpressionVisitor();
            SqlStatement sqlStatement = new SqlStatement(visitor);
            sqlStatement.Select = Expression.Constant("column1,column2,column4");
            sqlStatement.From = Expression.Constant("MyTable");
            sqlStatement.Where = bexp;
            // adding an expression
            sqlStatement.Where = bexp;
            string myStatement = sqlStatement.Render();

            // adding a new expression
            sqlStatement.Where = expExented;
            string nexSatement = sqlStatement.Render();

            string resultSql = visitor.Visit(expExented); 
        }
        [TestMethod]
        public void CheckBinaryExpression()
        {
            var exp = ExpressionExtensions.GetExpression<int>(5, 10);
            var toadd = ExpressionExtensions.GetExpression<int>(7, 15);
            var merged = ExpressionExtensions.AddNewExpression(exp, toadd);
            string result = merged.ToString();


        }
        [TestMethod]
        public void createCallExpression()
        {
            var param = Expression.Parameter(typeof(object), "self");
            var exp = Expression.Call(
                  param, typeof(object).GetMethod("ToString"));
           var result=  Expression.Lambda<Func<object,string>>(exp,param).Compile().Invoke("hello");
            
        }
        [TestMethod]
        public void CreateMuliptleBinaryExpressions()
        {
            ParameterExpression name = Expression.Parameter(typeof(string),"n");
            var b1 = Expression.MakeBinary(ExpressionType.Equal,
                Expression.Constant("value2"), name);
            var b2 = Expression.MakeBinary(ExpressionType.Equal,
               Expression.Constant("value2"), name);
               

            var joined = Expression.MakeBinary(ExpressionType.And,
                b1,
                b2
                );
            var secjoined = Expression.MakeBinary(ExpressionType.Or,
                joined,
                Expression.MakeBinary(ExpressionType.Equal,
               Expression.Constant("value2"), name));

            var l = Expression.Lambda<Func<string,bool>>(secjoined, name);

             

            List<string> names = new List<string>();
            names.Add("value2");
            var resul = names.Where((n) => l.Compile().Invoke(n)).ToList();
          

        }
        [TestMethod]
        public void MakeLambda()
        {
            var expleft = Expression.Constant("colName");
            var expRight = Expression.Constant("colValue");
             var lambda= Expression.Lambda<Func<bool>>(
                Expression.MakeBinary(ExpressionType.Equal,
                   expleft,
                   expRight
                ));
           var ok=  lambda.Compile().Invoke();
            MyExpressionVisitor visitor = new MyExpressionVisitor();
            var value= visitor.VisitBinary(lambda.Body as BinaryExpression);
           
        }

        [TestMethod]
        public void GetLambdaExpression()
        {
           var lexp = ExpressionExtensions.GetLambdaExp("ok", "ok");
            Func<string, string, bool> input = (Func<string, string, bool>)lexp.Compile();
            Expression<Func<string, string, bool>> expression = (arg1, arg2) => input(arg1, arg2);
            var result=  ExpressionExtensions.Invoke(expression);
            ExpressionExtensions.Invoke((arg1, arg2) => input(arg1, arg2));
        }
    }
        
    public static class ExpressionExtensions
    {
        public static BinaryExpression GetExpression<T>(int operLeft,int operRight)
        {
            return Expression<T>.MakeBinary(ExpressionType.Add,
                Expression.Constant(operLeft),
                Expression.Constant(operRight)
                );
        }
        public static BinaryExpression AddNewExpression(BinaryExpression exptoAdd,BinaryExpression existing)
        {
           
                return Expression.MakeBinary(ExpressionType.And,
                    existing,
                    exptoAdd
                    );
            
        }
        public static LambdaExpression GetLambdaExp(string left, string right)
        {
            ParameterExpression paramExpr = Expression.Parameter(typeof(string), "arg");
            ParameterExpression paramExpr2 = Expression.Parameter(typeof(string), "arg2");
            return Expression.Lambda<Func<string, string, bool>>(
                Expression.MakeBinary(ExpressionType.Equal,
                                      paramExpr,
                                      paramExpr2),
                                      paramExpr, paramExpr2);
                
        }
       
        public static LambdaExpression GetLambdaExp()
        {
            return Expression.Lambda(
                GetExpression<int>(10, 50));        }

        public static bool Invoke(Expression<Func<string,string,bool>> expression)
        {
            return expression.Compile().Invoke("ok", "ok");
        }
        public static void AccessProperty(Expression<Func<MyModel,bool>> expr)
        {
            var myl = expr;
        }
       
    }

    public class MyModel
    {
        public string Id { get; set; }
    }

    public class MyExpressionVisitor
    { 
        public MyExpressionVisitor() : base() { }
        public string Visit(Expression expression)
        {
            if(expression.NodeType==ExpressionType.And || expression.NodeType==ExpressionType.Or)
            {
                return VisitBinary(expression as BinaryExpression);
            }
            else if(expression.NodeType==ExpressionType.Equal)
            {
                return VisitBinary(expression as BinaryExpression);
            }
            else if(expression.NodeType==ExpressionType.Constant)
            {
                return VisitConstant(expression as ConstantExpression);
            }
            return "ok";
        }
        public string VisitBinary(BinaryExpression node)
        {
            return $"{this.Visit(node.Left )} { toSql(node.NodeType)} {this.Visit(node.Right)} ";
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
            if(expType==ExpressionType.And)
            {
                return "And";
            }
            else if(expType==ExpressionType.Or)
            {
                return "Or";
            }
            else if(expType==ExpressionType.NotEqual)
            {
                return "<>";
            }
            else if(expType==ExpressionType.Equal)
            {
                return "=";
            }
            else if(expType==ExpressionType.Coalesce)
            {
                return " ";
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
        public SqlStatement(MyExpressionVisitor visitor)
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
