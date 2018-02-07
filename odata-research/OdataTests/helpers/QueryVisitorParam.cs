using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace OdataTests.helpers
{
    public class QueryVisitorParam: QueryVisitor
    {
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();
        public QueryVisitorParam()
        {

        }
        public override string Visit(ConvertNode nodeIn)
        {
            string propertyName = base.Visit(nodeIn);
            return propertyName;
        }
        public override string Visit(ConstantNode nodeIn)
        {
            string paramValue= base.Visit(nodeIn);
            return paramValue;
        }
    }
}
