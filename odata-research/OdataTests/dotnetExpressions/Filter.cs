using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace OdataTests.dotnetExpressions
{
   public class FilterBuilder
    {
        private HashSet<string> _fieldNames = new HashSet<string>();
        private  dynamic _properties = new ExpandoObject();
        private StringBuilder _whereClause = new StringBuilder();
        private int paramIndex = 0;
        public FilterBuilder()
        {
            
        }
        internal void WriteToken(string tokenFormat,params string[] values)
        {
            this._whereClause.AppendFormat(tokenFormat, values);
        }
        public object ParamValues => _properties;

        public IReadOnlyCollection<string> FieldNames =>_fieldNames;
        
        public string ParametirizedWhereClause
        {
            get { return _whereClause.ToString(); }
        }
        internal void AddFieldName(string fieldName)
        {
            var fl = $"${fieldName}";
            _fieldNames.Add(fl);
            WriteToken("{0}",fl);
        }
        internal void InitAndSetParam(string paramName,object paramValue)
        {
            (_properties as IDictionary<string, object>)
                .Add(paramIndex.ToString(), paramValue);
            WriteToken("@{0}",paramIndex.ToString());
            paramIndex++;
    
        }
       
    }
}
