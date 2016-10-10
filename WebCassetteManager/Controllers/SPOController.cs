using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RDFCommon.OVns;

namespace WebCassetteManager.Controllers
{
    [RoutePrefix("spo")]
    public class SPOController : ApiController
    {
        [Route("sp/{subject}/{predicate}")]
        public IEnumerable<string> GetSP(string subject, string predicate)
        {
            return
                CasssettesBD.CasssettesBd.Store.GetTriplesWithSubjectPredicate(
                    new OV_iri(subject),
                    new OV_iri(predicate)).Select(variants => variants.ToString())
                    .ToArray();
        }

        [Route("po/{predicate}/{obj}")]
        public IEnumerable<string> GetPO(string predicate, string obj)
        {
            return
                CasssettesBD.CasssettesBd.Store.GetTriplesWithPredicateObject(
                    new OV_iri(predicate),
                    new OV_iri(obj)).Select(variants => variants.ToString())
                    .ToArray();

        }

        // GET: api/SPO/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SPO
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/SPO/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SPO/5
        public void Delete(int id)
        {
        }
    }
}
