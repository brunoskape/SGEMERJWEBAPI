using SGEMERJWEB.Bll;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.WebAPI.Model;
using SGEMERJWEB.WebAPI.Util;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SGEMERJWEB.WebAPI.Controllers
{
    [Authorize]
    public class CepController : ApiController
    {
        private readonly BllCEP bllCEP;

        public CepController()
        {
            bllCEP = new BllCEP();
        }

        [Route("api/cep/{cep}")]
        [HttpGet]
        public IHttpActionResult Obter(int cep)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            Endereco objeto = null;
            try
            {
                if (cep > 0 && cep.ToString().Length == 8)
                    objeto = bllCEP.ObterEnderecoPeloCEP(cep.ToString());

                if (objeto != null)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(0, 0, 0, objeto, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(0, 0, 0, objeto, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }

    }
}
