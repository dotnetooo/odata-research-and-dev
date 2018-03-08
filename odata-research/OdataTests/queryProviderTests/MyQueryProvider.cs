using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace OdataTests.queryProviderTests
{
    /// <summary>
    /// 
    /// </summary>
    class TerraServerQueryContext
    {
        // Executes the expression tree that is passed to it. 
        internal static object Execute(Expression expression, bool IsEnumerable)
        {
            // The expression must represent a query over the data source. 
            if (!IsQueryOverDataSource(expression))
                throw new InvalidProgramException("No query over the data source was specified.");

            // Find the call to Where() and get the lambda expression predicate.
            InnermostWhereFinder whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);
            LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            // Send the lambda expression through the partial evaluator.
            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            // Get the place name(s) to query the Web service with.
            LocationFinder lf = new LocationFinder(lambdaExpression.Body);
            List<string> locations = lf.Locations;
            if (locations.Count == 0)
                throw new InvalidQueryException("You must specify at least one place name in your query.");

            // Call the Web service and get the results.
            Place[] places = WebServiceHelper.GetPlacesFromTerraServer(locations);

            // Copy the IEnumerable places to an IQueryable.
            IQueryable<Place> queryablePlaces = places.AsQueryable<Place>();

            // Copy the expression tree that was passed in, changing only the first 
            // argument of the innermost MethodCallExpression.
            ExpressionTreeModifier treeCopier = new ExpressionTreeModifier(queryablePlaces);
            Expression newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods. 
            if (IsEnumerable)
                return queryablePlaces.Provider.CreateQuery(newExpressionTree);
            else
                return queryablePlaces.Provider.Execute(newExpressionTree);
        }

        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance, 
            // expression is of type ConstantExpression, not MethodCallExpression. 
            return (expression is MethodCallExpression);
        }
    }

/// <summary>
/// Data source of query. Implements i queryable.
/// </summary>
/// <typeparam name="TData"></typeparam>
     public class MyData<TData> : IQueryable<TData>
    {
        public MyData(MyQueryProvider provider, Expression expression)
        {
            if (!typeof(IQueryable<TData>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            Provider = provider;
            Expression = expression;
        }
        public MyData()
        {
            Provider = new MyQueryProvider();
            Expression = Expression.Constant(this);
        }
        public Type ElementType
        {
            get { return typeof(TData); }
        }

        public Expression Expression
        {
            get;
            private set;
        }

        public IQueryProvider Provider
        {
            get;
            private set;
        }

        public IEnumerator<TData> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<TData>>(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<System.Collections.IEnumerable>(Expression)).GetEnumerator();
        }
    }

    /// <summary>
    /// Query provider.
    /// </summary>
    public class MyQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(MyData<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MyData<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return TerraServerQueryContext.Execute(expression, false);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            bool IsEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            return (TResult)TerraServerQueryContext.Execute(expression, IsEnumerable);
        }
    }

    internal static class TypeSystem
    {
        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }

            return null;
        }
    }
}

