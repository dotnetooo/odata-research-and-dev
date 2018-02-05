using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.UriParser;
using Microsoft.AspNetCore.OData.Query;
using metadata;
using System.Web.OData.NHibernate;

namespace odata_research.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public ValuesController()
        {

        }
        // GET api/values
        [HttpGet]
        [EnableQuery]
        public string Get()
        {
            ///
            ///https://github.com/OData/WebApi/issues/33
            ///https://blogs.msdn.microsoft.com/alexj/2012/12/06/parsing-filter-and-orderby-using-the-odatauriparser/
            ///https://archive.codeplex.com/?p=aspnetwebstack#src%2fSystem.Web.Http.OData%2fOData%2fBuilder%2fODataConventionModelBuilder.cs
            Uri relativeUri = new Uri("Customers?$top = 2 &$skip = 0 &$orderby = Name desc, City asc &$filter = City eq 'Redmond'", UriKind.Relative);
            metadata.MetadataBuilder m = new MetadataBuilder();
            var model= m.BuildAddress().GetModel();
            ODataUriParser parser = new ODataUriParser(model,relativeUri);
            ODataPath path = parser.ParsePath();
            FilterClause filter = parser.ParseFilter();
            var odaUri = parser.ParseUri();
            NHibernateFilterBinder binder = new NHibernateFilterBinder(model);
            WhereClause where = NHibernateFilterBinder.GetWhere(filter, model);
            string arguments = string.Join(":", where.PositionalParameters);
            string clause = where.Clause;
            return clause.Replace("?", arguments);

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
