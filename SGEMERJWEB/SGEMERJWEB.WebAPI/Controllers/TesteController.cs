using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SGEMERJWEB.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SGEMERJWEB.WebAPI.Controllers
{
    //[Authorize]
    public class TesteController : ApiController
    {
   public TesteController()
        {
          
        }


        [Route("api/teste")]
        [HttpGet]
        public IHttpActionResult Teste()
        {
            Request.Headers.TryGetValues("thecodebuzz", out var traceValue);

            return Ok(traceValue);

        }


    }
}

