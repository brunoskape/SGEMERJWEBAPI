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
    public class TipoBeneficioController : ApiController
    {
        private BllTipoBeneficio bllTipoBeneficio;

        [Route("api/tipobeneficio")]
        [HttpGet]
        public IHttpActionResult ObterLista(string d = "", int p = 0)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                bllTipoBeneficio = new BllTipoBeneficio(null);
                var resultado = bllTipoBeneficio.RetornarTipoBeneficios(d,  p);
                var lstTipoBeneficio = resultado.Item4;
                var totalPagina = resultado.Item2;
                var totalRegistro = resultado.Item3;
                var pagina = resultado.Item1;

                if (lstTipoBeneficio != null && lstTipoBeneficio.Any())
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(totalPagina, totalRegistro, pagina, lstTipoBeneficio, Constante.P_STATUS_OK)));
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


        [Route("api/tipobeneficio/{id}")]
        [HttpGet]
        public IHttpActionResult Obter(int id)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                bllTipoBeneficio = new BllTipoBeneficio(null);
                var tipoBeneficio = bllTipoBeneficio.RetornarTipoBeneficio(id);

                if (tipoBeneficio != null)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, tipoBeneficio, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, tipoBeneficio, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }

        [Route("api/tipobeneficio")]
        [HttpPost]
        public IHttpActionResult Incluir(TipoBeneficio tipoBeneficio)
        {
            return Gravar(tipoBeneficio);
        }

        [Route("api/tipobeneficio")]
        [HttpPut]
        public IHttpActionResult Alterar(TipoBeneficio tipoBeneficio)
        {
            return Gravar(tipoBeneficio);
        }


        private IHttpActionResult Gravar(TipoBeneficio tipoBeneficio)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                bllTipoBeneficio = new BllTipoBeneficio(TokenService.GetCodUsu(User.Identity as ClaimsIdentity));
                var msg = bllTipoBeneficio.GravarTipoBeneficio(tipoBeneficio);

                var retorno = RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_OK, msg);

                if (msg.ToUpper().Equals("ALTERADO") || msg.ToUpper().Equals("INCLUIDO"))
                {
                    retorno.Data = tipoBeneficio;
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
