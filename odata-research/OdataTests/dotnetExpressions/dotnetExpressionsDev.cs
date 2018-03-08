using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace OdataTests.dotnetExpressions
{
    [TestClass]
   public  class dotnetExpressionsDev
    {
        [TestMethod]
        public void CheckBinaryExpression()
        {
            var exp = ExpressionExtensions.GetExpression<int>(5, 10);
            var toadd = ExpressionExtensions.GetExpression<int>(7, 15);
            var merged = ExpressionExtensions.AddNewExpression(exp, toadd);
            string result = merged.ToString();


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
        public string VisitBinary(BinaryExpression node)
        {
            return $"{(node.Left as ConstantExpression).Value.ToString()}={(node.Right as ConstantExpression).Value.ToString()}";
        }
         public string VisitConstant(ConstantExpression node)
        {

            return node.Value.ToString();
        }
        public string VisitParameter(ParameterExpression node)
        {
         
            return node.Name;
        }
    }
  
}
