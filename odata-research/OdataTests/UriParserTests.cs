using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData;
using System;

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

        private static IEdmModel getModel()
        {
            return new MetadataBuilder()
                .BuildCustomer()
                .GetModel();
        }
    }
}
