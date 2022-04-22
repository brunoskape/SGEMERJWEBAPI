using SGEMERJWEB.Bll;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.WebAPI.Model;
using SGEMERJWEB.WebAPI.Service;
using SGEMERJWEB.WebAPI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace SGEMERJWEB.WebAPI.Controllers
{
    //[Authorize]
    public class RescisaoController : ApiController
    {

        private BllRescisao _bllRescisao;

        public RescisaoController()
        {
            _bllRescisao = new BllRescisao(TokenService.GetCodUsu(User.Identity as ClaimsIdentity),
                                         TokenService.GetIdUsu(User.Identity as ClaimsIdentity));
        }

        //Obter o colaborador com seus dados de rescisão, o parametro value na assinatura do método é o id do colaboradorTipo. 
        [Route("api/rescisao/{id}")]
        [HttpGet]
        public IHttpActionResult ObterRescisaoPorId(string id)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                var retorno = _bllRescisao.ObterRescisaoPorId(id);

                if (retorno != null)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, retorno, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }

        [Route("api/rescisao")]
        [HttpPost]
        public IHttpActionResult Incluir(Colaborador objeto)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                var retorno = _bllRescisao.Incluir(objeto);

                if (retorno == "INCLUIDO")
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, retorno, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK)));
                }
            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }


        [Route("api/rescisao")]
        [HttpPut]
        public IHttpActionResult Alterar(Colaborador objeto)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                var retorno = _bllRescisao.Alterar(objeto);

                if (retorno == "ALTERADO")
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, retorno, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK)));
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