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
   // [Authorize]
    public class ColaboradorController : ApiController
    {
        private BllColaborador objetoBLL;        

        [Route("api/colaboradores")]
        [HttpGet]
        public IHttpActionResult ObterLista(string n = "", string c = "", int t = 0, int p = 1)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                objetoBLL = new BllColaborador(null);
                var retorno = objetoBLL.ObterListaPaginada(n, c, t, p, Constante.P_TOTAL_REGISTROS_POR_PAGINA);

                if (retorno.lista != null && retorno.lista.Any())
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(retorno.totalPaginas, retorno.totalRegistros, p, retorno.lista, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(retorno.totalPaginas, retorno.totalRegistros, p, null, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }


        [Route("api/colaboradores/{idDoTipo}")]
        [HttpGet]
        public IHttpActionResult Obter(int idDoTipo)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            
            try
            {
                objetoBLL = new BllColaborador(null);
                var colaborador = objetoBLL.Obter(idDoTipo, true );

                if (colaborador != null)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, colaborador, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, null, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }

        [Route("api/colaboradores/{tipo}/{cpf}")]
        [HttpGet]
        public IHttpActionResult Obter(int tipo, string cpf)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;


            try
            {
                objetoBLL = new BllColaborador(null);
                var colaborador = objetoBLL.ObterPorTipoECPF(tipo,cpf, true);

                if (colaborador != null)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, colaborador, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, null, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }

        [Route("api/colaboradores")]
        [HttpPost]
        public IHttpActionResult Incluir(Colaborador objeto)
        {
            return Gravar(objeto);
        }

        [Route("api/colaboradores")]
        [HttpPut]
        public IHttpActionResult Alterar(Colaborador objeto)
        {
            return Gravar(objeto);
        }


        private IHttpActionResult Gravar(Colaborador objeto)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                objetoBLL = new BllColaborador(TokenService.GetCodUsu(User.Identity as ClaimsIdentity),
                                               TokenService.GetIdUsu(User.Identity as ClaimsIdentity));
                var msg = objetoBLL.Gravar(objeto);

                var retorno = RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_OK, msg);

                if (msg.ToUpper().Equals("ALTERADO") || msg.ToUpper().Equals("INCLUIDO") || msg.ToUpper().Equals("INCLUIDO-TIPO"))
                {
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
