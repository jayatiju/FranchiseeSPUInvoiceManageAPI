using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class ValuesController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        //{
            //eturn new string[] { "value1", "value2" };
        //}

        // GET api/values/
        [HttpGet]
        public string Demo(string branchcode, string vendorcode)
        {
            string result = $"Branchcode : {branchcode}, Vendorcode: {vendorcode}";
            return result;
        }
        

        // POST api/values
        //public void Post([FromBody] string value)
        //{
        //}

        /*
        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
        */
    }
}
