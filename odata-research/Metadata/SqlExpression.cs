using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
namespace Metadata
{
   public class SqlExpression:Expression
    {
       
        public SqlExpression(params Expression[] expressions):base()
        {
            
        }
        public override ExpressionType NodeType => ExpressionType.Constant;
        public override Type Type => typeof(string);
        public new virtual  Expression Accept(ExpressionVisitor visitor)
        {
            return Expression.Constant("SELECT");
        }
        

    }
    //public class Select:SqlExpression
    //{
    //    public Select():base()
    //    {

    //    }
    //    protected override Expression Accept(ExpressionVisitor visitor)
    //    {
    //        return Expression.Constant("SELECT", typeof(string));
    //    }
    //}
}
