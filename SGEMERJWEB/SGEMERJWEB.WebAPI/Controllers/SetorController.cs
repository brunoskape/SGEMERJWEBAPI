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
    [Authorize]
    public class SetorController : ApiController
    {
        private BllSetor bllSetor;

        [Route("api/setores")]
        [HttpGet]
        public IHttpActionResult ObterLista(string d = "", string s = "", int p = 0)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                bllSetor = new BllSetor(null);
                var resultado = bllSetor.RetornarSetores(d, s, p);
                var lstSetor = resultado.Item4;
                var totalPagina = resultado.Item2;
                var totalRegistro = resultado.Item3;
                var pagina = resultado.Item1;

                if (lstSetor != null && lstSetor.Any())
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(totalPagina, totalRegistro, pagina, lstSetor, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(totalPagina, totalRegistro, pagina, null, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }


        [Route("api/setores/{id}")]
        [HttpGet]
        public IHttpActionResult Obter(int id)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                bllSetor = new BllSetor(null);
                var setor = bllSetor.RetornarSetor(id);

                if (setor != null)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, setor, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, setor, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }

        [Route("api/setores")]
        [HttpPost]
        public IHttpActionResult Incluir(Setor setor)
        {
            return Gravar(setor);
        }

        [Route("api/setores")]
        [HttpPut]
        public IHttpActionResult Alterar(Setor setor)
        {
            return Gravar(setor);
        }


        private IHttpActionResult Gravar(Setor setor)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                bllSetor = new BllSetor(TokenService.GetCodUsu(User.Identity as ClaimsIdentity));
                var msg = bllSetor.GravarSetor(setor);

                var retorno = RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_OK, msg);

                if (msg.ToUpper().Equals("ALTERADO") || msg.ToUpper().Equals("INCLUIDO"))
                {
                    retorno.Data = setor;
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, retorno));
                }
                else
                {
                    retorno.Status = Constante.P_STATUS_NOK;
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, retorno));
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
