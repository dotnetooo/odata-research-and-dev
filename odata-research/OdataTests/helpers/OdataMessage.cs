using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class OdataMessage : IODataResponseMessage
{
    private MemoryStream _stream;
    private int _statusCode;
    public OdataMessage(MemoryStream stream)
    {
        _stream = stream;
    }
    public IEnumerable<KeyValuePair<string, string>> Headers => null;

    public int StatusCode { get => _statusCode; set =>_statusCode=value; }

    public string GetHeader(string headerName)
    {
        return "";
    }

    public Stream GetStream()
    {
        return _stream;
    }

    public void SetHeader(string headerName, string headerValue)
    {
        
    }
}

