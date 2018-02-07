using Microsoft.OData.UriParser;
using System.Text;

public static class VisitorExtensions
    {
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

