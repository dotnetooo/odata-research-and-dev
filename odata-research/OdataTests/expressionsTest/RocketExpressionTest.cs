using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace OdataTests.expressionsTest
{
    [TestClass]
   public class RocketExpressionTest
    {
        [TestMethod]
        public void buildTree()
        {
            ExpressionsTree tree = new ExpressionsTree();
            tree.From()
                .Where()
                .Select();
        }
    }
}
