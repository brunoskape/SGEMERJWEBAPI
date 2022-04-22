using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using HttpDeleteAttribute = System.Web.Http.HttpDeleteAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPatchAttribute = System.Web.Http.HttpPatchAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using HttpPutAttribute = System.Web.Http.HttpPutAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace SGEMERJWEB.WebAPI.Controllers
{
    public class HomeController : ApiController
    {
        [Route("")]
        [Route("api")]
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [HttpPatch]
        public IHttpActionResult Index()
        {
            var versao = WebConfigurationManager.AppSettings["VERSAOSISTEMA"];
            versao += " - 20210611121400";
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "API SGEMERJWEB - " + versao));
        }
    }
}
