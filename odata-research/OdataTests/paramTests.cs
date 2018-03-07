using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace OdataTests
{
    [TestClass]
   public class paramTests
    {

        [TestMethod]
        public void createParameters()
        {
            ParamCollection collection = new ParamCollection();
            collection.Add(new SqlParam() { Name = "@firstName", Value = "john" });
            collection.Add(new SqlParam() { Name = "@firstName", Value = "john" });
            Assert.IsTrue(collection.Count == 2);
            Assert.IsTrue(collection.Find(p => p.Name == "@firstName1") != null);
            var paramValues = collection.ParameterValues;
            var paramKeys = collection.ParameterKeys;
            Assert.IsTrue(2 == paramValues.Length);
        }
    }
}
