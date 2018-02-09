using Microsoft.OData.UriParser;
using System.Linq.Expressions;
using System.Text;

public static class VisitorExtensions
    {
    
    //public Expression ToExpression(this QueryNode query)
    //{
    //    switch(query)
    //    {
    //        case SingleValueNode singleValueNode:
    //            break;
    //        case NamedFunctionParameterNode functionParamNode:
    //            break;
    //        case CollectionNode collectionNode:
    //            break;
    //        case QueryNode queryNode:
    //            break;
           
    //    }
    //}
    public static ExpressionType ToExpressionKind(this BinaryOperatorKind binaryOperator)
    {
        switch (binaryOperator)
        {
            case BinaryOperatorKind.Add:
                return ExpressionType.Add;

            case BinaryOperatorKind.And:
                return ExpressionType.And;

            case BinaryOperatorKind.Divide:
                return ExpressionType.Divide;

            case BinaryOperatorKind.Equal:
                return ExpressionType.Equal;

            case BinaryOperatorKind.GreaterThan:
                return ExpressionType.GreaterThan;

            case BinaryOperatorKind.GreaterThanOrEqual:
                return ExpressionType.GreaterThanOrEqual;

            case BinaryOperatorKind.LessThan:
                return ExpressionType.LessThan;

            case BinaryOperatorKind.LessThanOrEqual:
                return ExpressionType.LessThanOrEqual;

            case BinaryOperatorKind.Modulo:
                return ExpressionType.Modulo;

            case BinaryOperatorKind.Multiply:
                return ExpressionType.Multiply;

            case BinaryOperatorKind.NotEqual:
                return ExpressionType.NotEqual;

            case BinaryOperatorKind.Or:
                return ExpressionType.Or;

            case BinaryOperatorKind.Subtract:
                return ExpressionType.Subtract;
            default:
                return default(ExpressionType);
        }
    }
     public static string ToSqlOperator(this BinaryOperatorKind binaryOpertor)
    {
        switch (binaryOpertor)
        {
            case BinaryOperatorKind.Add:
                return "+";
            case BinaryOperatorKind.And:
                return "AND";
            case BinaryOperatorKind.Divide:
                return "/";
            case BinaryOperatorKind.Equal:
                return "=";
            case BinaryOperatorKind.GreaterThan:
                return ">";
            case BinaryOperatorKind.GreaterThanOrEqual:
                return ">=";
            case BinaryOperatorKind.LessThan:
                return "<";
            case BinaryOperatorKind.LessThanOrEqual:
                return "<=";
            case BinaryOperatorKind.Modulo:
                return "%";
            case BinaryOperatorKind.Multiply:
                return "*";
            case BinaryOperatorKind.NotEqual:
                return "!=";
            case BinaryOperatorKind.Or:
                return "OR";
            case BinaryOperatorKind.Subtract:
                return "-";
            default:
                return null;
        }
    }
     public static string ToSqlOperator(this UnaryOperatorKind unaryOperator)
    {
            switch (unaryOperator)
            {
                case UnaryOperatorKind.Negate:
                    return "!";
                case UnaryOperatorKind.Not:
                    return "NOT";
                default:
                    return null;
             }
    }
     public static string ToSqlOrderBy(this OrderByClause orderClause)
    {
        StringBuilder builder = new StringBuilder("Order By ");
        while (orderClause != null)
        {
            builder.AppendFormat("{0} {1}", (orderClause.Expression as SingleValuePropertyAccessNode)?.Property.Name,
                                          orderClause.Direction == OrderByDirection.Ascending ? "ASC" : "DESC");
            orderClause = orderClause.ThenBy;
            if (null != orderClause) { builder.Append(","); }
        }
        return builder.ToString();
    }
     public static string ToSqlWhereClause(this FilterClause filterClause)
    {
        QueryVisitor visitor = new QueryVisitor();
        return filterClause.Expression.Accept<string>(visitor);
    }
}

