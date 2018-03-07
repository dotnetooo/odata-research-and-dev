using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

    public sealed class ParamCollection : List<SqlParam>
    {
        public ParamCollection() : base() { }

        public new string Add(SqlParam sqlParam)
        {
        if (null == sqlParam) throw new ArgumentNullException(nameof(sqlParam));
          var group = base.FindAll(p => p.Name.Contains(sqlParam.Name));
          int postfix = (0 == group?.Count) ? 0 : group.Count;
          sqlParam.Name = $"{sqlParam.Name}{postfix}";
          base.Add(sqlParam);
          return sqlParam.Name;
        }

       public object[] ParameterValues => this.Select(p => p.Value).ToArray();
        public string[] ParameterKeys => this.Select(p => p.Name).ToArray();

}

