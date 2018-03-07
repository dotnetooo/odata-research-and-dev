using System;

public sealed class SqlParam
    {
        public string Name
        {
            get;
            set;
        }
        public object Value
        {
            get;
            set;
        }
    public override bool Equals(Object obj)
    {
        SqlParam right = obj as SqlParam;
        return right == null ? false :
               this.Equals(right.Name);
        
    }
    public override int GetHashCode()
    {
        return this.Name.GetHashCode();
    }

}

