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
using Microsoft.OData;

namespace odata_research.Controllers
{
    [Route("api/[controller]")]
   {
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public ValuesController()
        {

        }
        // GET api/values
        [HttpGet]
        [Route("Customers")]
        public string Get()
        {
            Uri relativeUri = new Uri($"Customers?{this.HttpContext.Request.QueryString.Value}", UriKind.Relative);
            ODataUriParser parser = new ODataUriParser(
                new MetadataBuilder()
                .BuildCustomer()
                .GetModel(),
                relativeUri);
            ODataUri oDataUri = parser.ParseUri();
            string where = oDataUri.Filter.ToSqlWhereClause();
            string orderBy = oDataUri.OrderBy?.ToSqlOrderBy();
            return $"{ where} {orderBy}";

        }

        // GET api/values/5
        [HttpGet]
        [Route("$metadata")]
        public string GetMetadata()
        {
            IEdmModel model = new MetadataBuilder().BuildCustomer().GetModel();
            MemoryStream stream = new MemoryStream();
            OdataMessage message = new OdataMessage(stream);
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            ODataMessageWriter writer = new ODataMessageWriter((IODataResponseMessage)message, settings, model);
            writer.WriteMetadataDocument();
            return Encoding.UTF8.GetString(stream.ToArray());

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
}
