using System;
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
    }
}
