using System.Collections.Generic;

namespace OdataTests.helpers
{
    public interface ILexer<T> where T:class
    {
        T GetNode();
    }
}