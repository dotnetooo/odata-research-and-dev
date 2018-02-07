using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData;

   public sealed  class QueryVisitor:QueryNodeVisitor<string>
    {
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
        string result = string.Empty;
        if (nodeIn.OperatorKind != BinaryOperatorKind.Equal)
        {
            result= $"{nodeIn.Left.Accept<string>(this)} {nodeIn.OperatorKind.ToSqlOperator()} { nodeIn.Right.Accept<string>(this)}";
        }
        else
        {
            if(nodeIn.Right.Kind==QueryNodeKind.Constant)
            {
                string condition = nodeIn.Right.Accept<string>(this);
                if(condition.Contains(";"))
                {
                    var values = condition.Split(new char[] { ';' });
                    result= $"{nodeIn.Left.Accept<string>(this)} IN ( {string.Join(",",values)} )";
                }
                else
                {
                    result = $"{nodeIn.Left.Accept<string>(this)} {nodeIn.OperatorKind.ToSqlOperator()} { nodeIn.Right.Accept<string>(this)}";
                }
            }
        }
        return result;
        }
        public override string Visit(UnaryOperatorNode nodeIn)
        {
            return base.Visit(nodeIn);
        }
        public override string Visit(SingleValuePropertyAccessNode nodeIn)
        {
            return base.Visit(nodeIn);
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
          return (nodeIn.Source as Microsoft.OData.UriParser.SingleValuePropertyAccessNode)?.Property?.Name;
        }
        public override string Visit(ConstantNode nodeIn)
        {
           return nodeIn.LiteralText;
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
    }

