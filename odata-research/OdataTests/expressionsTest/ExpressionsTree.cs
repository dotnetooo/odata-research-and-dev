using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
namespace OdataTests.expressionsTest
{
   public  class ExpressionsTree
    {
        Expression _node;
        public ExpressionsTree()
        {

           
        }
        internal void Add(Expression node)
        {
            _node = node;
        }
        
    }
    public class Select : Expression
    {
        public Select() : base()
        {

        }
    }
    public class From : Expression
    {
        public From() : base()
        {

        }
    }
    public class Where : Expression
    {
        public Where() : base()
        {

        }
    }
    public static class ExpressionTreeExtensions
    {

        public static ExpressionsTree From(this ExpressionsTree theObject)
        {
            return theObject;
        }
        public static ExpressionsTree Where(this ExpressionsTree theObject)
        {
            return theObject;
        }
        public static ExpressionsTree Select(this ExpressionsTree theObject)
        {
            return theObject;
        }
      
    }
   
}
