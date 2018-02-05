using Microsoft.OData.UriParser;

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
}

