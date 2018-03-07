using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
namespace Metadata
{
   public  class SqlVisitor: ExpressionVisitor
    {
        public SqlVisitor() : base() { }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            return base.VisitConstant(node);
        }
    }
}
