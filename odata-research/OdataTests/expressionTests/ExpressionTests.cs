using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Metadata;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
namespace OdataTests.expressionTests
{
    [TestClass]
    public class ExpressionTests
    {

        [TestMethod]
        public void createSelect()
        {

            SqlExpression exp = new SqlExpression();
            SqlVisitor visitor = new SqlVisitor();
            var result=  exp.Accept(visitor);
            var value = (result as ConstantExpression).Value;
            var obj= (object)null;

        }
    }
    
}
