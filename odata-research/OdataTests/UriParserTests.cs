using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData;
using System;
using OdataTests.helpers;
using System.Linq.Expressions;

namespace OdataTests
{
    [TestClass]
    public class UriParserTests
    {
        Uri relativeUri = new Uri("Customers?$top = 2 &$skip = 0 &$orderby = Name desc, City asc &$filter = City eq 'Redmond' and Name eq 'customerName' or State eq 'ut'", UriKind.Relative);
        [TestMethod]
        public void parseFilter_query_optionsOnly()
        {
            ODataUriParser parser = new ODataUriParser(getModel(), relativeUri);
            ODataUri oDataUri= parser.ParseUri();
            QueryVisitor visitor = new QueryVisitor();
            string data= oDataUri.Filter.Expression.Accept<string>(visitor);
        }
        [TestMethod]
        public void parseOrderBy_query()
        {
            ODataUriParser parser = new ODataUriParser(getModel(), relativeUri);
            ODataUri oDataUri = parser.ParseUri();
            QueryVisitor visitor = new QueryVisitor();
            string orderBy = oDataUri.OrderBy.ToSqlOrderBy();
        }
        [TestMethod]
        public void parserFilter_expression()
        {
            ODataUriParser parser = new ODataUriParser(getModel(), relativeUri);
            ODataUri oDataUri = parser.ParseUri();
            QueryVistorExpression visitor = new QueryVistorExpression();
            var expression= oDataUri.Filter.Expression.Accept<Expression>(visitor);
            string value = expression.ToString();
           
        }
        private static IEdmModel getModel()
        {
            return new MetadataBuilder()
                .BuildCustomer()
                .GetModel();
        }
    }
}
