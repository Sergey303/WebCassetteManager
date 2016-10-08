using System.Collections.Generic;
using System.Web.Http;

namespace WebCassetteManager.Controllers
{
    public class EditController : ApiController
    {
        // GET: api/Edit
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Edit/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Edit
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Edit
        public void Put([FromBody]string subject, [FromBody]string predicate, [FromBody]string @object)
        {

        }

        // DELETE: api/Edit/5
        public void Delete(int id)
        {
        }
    }
}
