using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ResourceApplicationTool.App_Start
{
    public class dddController : ApiController
    {
        // GET: api/ddd
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ddd/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ddd
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ddd/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ddd/5
        public void Delete(int id)
        {
        }
    }
}
