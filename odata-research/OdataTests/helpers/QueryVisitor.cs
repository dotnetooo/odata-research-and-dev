using Microsoft.OData.UriParser;
using System.Linq;
public sealed  class QueryVisitor:QueryNodeVisitor<string>
    {
    private readonly ParamCollection Parameters = new ParamCollection();
        public override string Visit(AllNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(AnyNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(CollectionComplexNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(BinaryOperatorNode nodeIn)
        {
          return $"{nodeIn.Left.Accept<string>(this)} {nodeIn.OperatorKind.ToSqlOperator()} { nodeIn.Right.Accept<string>(this)}";

        }
        public override string Visit(UnaryOperatorNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(SingleValuePropertyAccessNode nodeIn)
        {
        return nodeIn.Property.Name;
        }
        public override string Visit(SingleValueOpenPropertyAccessNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(SingleValueFunctionCallNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(SingleValueCastNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(SingleResourceFunctionCallNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(SingleResourceCastNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(SingleNavigationNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(SingleComplexNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(SearchTermNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(ResourceRangeVariableReferenceNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(ParameterAliasNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(NonResourceRangeVariableReferenceNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(NamedFunctionParameterNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(CountNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(ConvertNode nodeIn)
        {
          var property=(nodeIn.Source as SingleValuePropertyAccessNode)?.Property?.Name;
         Parameters.Add(new SqlParam() { Name = $"@{property}" });
         return property;
        }
        public override string Visit(ConstantNode nodeIn)
        {
          var param= Parameters.Last();
          param.Value = nodeIn.Value;
          return param.Name;
        }
        public override string Visit(CollectionResourceFunctionCallNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(CollectionResourceCastNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(CollectionPropertyAccessNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(CollectionOpenPropertyAccessNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(CollectionNavigationNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(CollectionFunctionCallNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
       public string[] ParametersKeys => Parameters.ParameterKeys;
      public object[] ParameterValues => Parameters.ParameterValues;
   

    }

