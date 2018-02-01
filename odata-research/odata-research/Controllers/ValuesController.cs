using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.UriParser;
using metadata;
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
        public IEnumerable<string> Get()
        {
            Uri relativeUri = new Uri("Address?select=Street",UriKind.Relative);
            metadata.MetadataBuilder m = new MetadataBuilder();
            var model= m.BuildAddress().GetModel();
            ODataUriParser parser = new ODataUriParser(model,relativeUri);
            ODataPath path = parser.ParsePath();
            return new string[] { "value1", "value2" };
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
