using Microsoft.IdentityModel.Tokens;
using SGEMERJWEB.Bll;
using SGEMERJWEB.WebAPI.Model;
using SGEMERJWEB.WebAPI.Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;

namespace SGEMERJWEB.WebAPI.Controllers
{
    public class AcessoController : ApiController
    {
        private readonly TokenService tokenService;
        private readonly BllAcesso bllAcesso;

        public AcessoController()
        {
            var sigla = WebConfigurationManager.AppSettings["siglaSistema"];
            var strAmbiente = WebConfigurationManager.AppSettings["AmbienteApp"];
            var urlValida = WebConfigurationManager.AppSettings["urlValidaPermissao." + strAmbiente];

            tokenService = new TokenService();                      

            bllAcesso = new BllAcesso(sigla, urlValida, strAmbiente);
        }

        [Route("api/acesso/ObterToken/{SEGSESSIONID}/{CODORG}")]
        [HttpGet]
        public IHttpActionResult ObterToken(string SEGSESSIONID, string CODORG)
        {
            var tokenModel = new TokenModel();
            try
            {
                var usuario = bllAcesso.ObterUsuarioComSessionID(SEGSESSIONID, CODORG);
                if (usuario != null)
                {
                    tokenModel = tokenService.Get(usuario.codUsu, usuario.idUsu.ToString());
                    if (tokenModel != null)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, tokenModel));
                    }
                    else
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, tokenModel));
                    }
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, new { status = "NOK", data = "Acesso negado" }));
                }
            }
            catch
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, tokenModel));
            }

        }

        [Route("api/acesso/ObterUsuarioComSessionID/{SEGSESSIONID}/{CODORG}")]
        [HttpGet]
        public IHttpActionResult ObterUsuarioComSessionID(string SEGSESSIONID, string CODORG)
        {

            try
            {                

                var usuario = bllAcesso.ObterUsuarioComSessionID(SEGSESSIONID, CODORG);              

                if (usuario != null)
                {
                    var tokenModel = tokenService.Get(usuario.codUsu, usuario.idUsu.ToString());
                    if (tokenModel != null)
                    {
                        usuario.Token.Parse(tokenModel.TokenJWT, tokenModel.Expiration);
                    }

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, usuario));
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, new { status = "NOK", data = "Acesso negado" }));
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
            }

        }

    }
}
