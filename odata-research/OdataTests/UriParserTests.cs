using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData;
using System;
using OdataTests.helpers;
using System.Linq.Expressions;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.OData.UriParser.Aggregation;
using System.Reflection;

namespace OdataTests
{
    [TestClass]
    public class UriParserTests
    {

        Uri relativeUri = new Uri("Customers?$top = 2 &$skip = 0 &$orderby = Name desc, City asc &$filter = City eq 'Redmond' and Name eq 'customerName' or State eq 'ut' and State ne 'uta'", UriKind.Relative);


        #region OdataQueryParser
        [TestMethod]
        public void parseFilter()
        {
            string filterStr = "City eq 'Redmond' and Name eq 'customerName' or Id eq 9.80000360";
            string orderBy = "Rating,Category/Name desc";
            UriQueryExpressionParser parser = new UriQueryExpressionParser(50);
            QueryToken token=  parser.ParseFilter(filterStr);
            NodeVisitor visitor = new NodeVisitor();
            string where= token.Accept<string>(visitor);
            IEnumerable<OrderByToken> tokens = typeof(UriQueryExpressionParser)
                                              .GetMethod("ParseOrderBy", BindingFlags.NonPublic | BindingFlags.Instance)
                                              .Invoke(parser, new object[] { orderBy })
                                               as IEnumerable<OrderByToken>;
        }
        #endregion



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
        [TestMethod]
        public  void parseExpression()
        {
            List<User> source = new List<User>();
            source.Add(new User() { Name="ruslan"});
            source.Add(new User());
            User target = new User() { Name = "ruslan" };
            ParameterExpression userParam = Expression.Parameter(typeof(User), "Username");// lambda expression parameter

            MemberExpression memberExpression = Expression.Property(userParam, "Name");

            ConstantExpression property = Expression.Constant("ruslan");

            BinaryExpression body = Expression.Equal(memberExpression, property);

            LambdaExpression filter = Expression.Lambda(body, userParam);

            Expression<Func<User, bool>> expFilter = Expression.Lambda<Func<User, bool>>(body, userParam);

            var result = source.Where((U) =>
            {
                var ok = filter.Compile().DynamicInvoke(U) as bool?;
                return ok.Value;
            }).ToList();
            // or
            result = source.Where(u => (filter.Compile().DynamicInvoke(u) as bool?).Value).ToList();

            Expression<Func<User, bool>> expression = user => user.Name == "UserName";
            result= source.Where(expression.Compile()).ToList();
            BinaryExpression expr = expression.Body as BinaryExpression;
            if (expr.NodeType == ExpressionType.Equal)
            {
                Expression objecAccess = expr.Left;
                Expression value = expr.Right;
             
            }
          
        }
       
        [TestMethod]
        public void parseGenericExpression()
        {
            List<User> source = new List<User>();
            source.Add(new User() { Name = "ruslan" });
            source.Add(new User());
            var filter = createGenericExpression<User>("ruslan", "Name");
           var result = source.Where(u => (filter.Compile().DynamicInvoke(u) as bool?).Value).ToList();
            Assert.IsTrue(1 == result.Count);
        }

        [TestMethod]
        public void testExpressionVisitor()
        {
            //arrange
            List<Customer> source = new List<Customer>();
            source.Add(new Customer() { Name = "ruslan" });
            source.Add(new Customer() { Name = "customerName", City="Redmond" });
            ODataUriParser parser = new ODataUriParser(getModel(), relativeUri);
            ODataUri oDataUri = parser.ParseUri();
            var lambda = oDataUri.Filter.ToLambda<Customer>()?.Compile();
            var result = source.Where(m => (lambda.DynamicInvoke(m) as bool?).Value);
            Assert.IsTrue(1 == result.Count());
            
        }
       private LambdaExpression createGenericExpression<TModel>(string propertyValue,string propertyName)
        {
            ParameterExpression userParam = Expression.Parameter(typeof(TModel), "Model");// lambda expression parameter

            MemberExpression memberExpression = Expression.Property(userParam, propertyName);

            ConstantExpression property = Expression.Constant(propertyValue);

            BinaryExpression body = Expression.Equal(memberExpression, property);

            LambdaExpression filter = Expression.Lambda(body, userParam);
            return filter;
        }
    }


    public class User
    {
        public string ID
        {
            get; set;
        }
        public string Name
        {
            get;
            set;
        }
    }
}
