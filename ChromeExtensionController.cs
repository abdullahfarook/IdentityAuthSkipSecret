using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("ExtensionPolicy")]
    public class ChromeExtensionController : ControllerBase
    {
        // GET: api/ChromeExtension
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ChromeExtension/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ChromeExtension
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/ChromeExtension/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ChromeExtension/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
