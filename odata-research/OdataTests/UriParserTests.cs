using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData;
using System;
using OdataTests.helpers;
using System.Linq.Expressions;
using System.Linq;
using System.Diagnostics;

namespace OdataTests
{
    [TestClass]
    public class UriParserTests
    {
        Uri relativeUri = new Uri("Customers?$top = 2 &$skip = 0 &$orderby = Name desc, City asc &$filter = City eq 'Redmond' and Name eq 'customerName' or State eq 'ut' and State ne 'uta'", UriKind.Relative);
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
            var expressionOrderBy = oDataUri.OrderBy.ToExpression();
            
            string value = expression.ToString();
           
        }
        [TestMethod]
        public void queryData()
        {
            ODataUriParser parser = new ODataUriParser(getModel(), relativeUri);
            ODataUri oDataUri = parser.ParseUri();
            QueryVistorExpression visitor = new QueryVistorExpression();
            var expression = oDataUri.Filter.Expression.Accept<Expression>(visitor);
            IQueryable query = new[] { "salt lake", "new your" }.AsQueryable();
            var source = query.Provider.CreateQuery(expression);
        }
        [TestMethod]
        public void createMethodCallExpression()
        {
            IQueryable<string> data = new[] { "salt lake city", "new your" }
                            .AsQueryable<string>();
            ParameterExpression pe = Expression.Parameter(typeof(string), "company");
            Expression left = Expression.Call(pe, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
            Expression right = Expression.Constant("coho winery");
            Expression e1 = Expression.Equal(left, right);

            // Create an expression tree that represents the expression 'company.Length > 16'.  
            left = Expression.Property(pe, typeof(string).GetProperty("Length"));
            right = Expression.Constant(16, typeof(int));
            Expression e2 = Expression.GreaterThan(left, right);

            Expression predicateBody = Expression.OrElse(e1, e2);
            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { data.ElementType },
                data.Expression,
                Expression.Lambda<Func<string, bool>>(predicateBody, new ParameterExpression[] { pe }));

            IQueryable<string> results = data.Provider.CreateQuery<string>(whereCallExpression);
            foreach(string s in results)
            {
                Debug.WriteLine(s);
            }


        }
        private static IEdmModel getModel()
        {
            return new MetadataBuilder()
                .BuildCustomer()
                .GetModel();
        }
    }
}
