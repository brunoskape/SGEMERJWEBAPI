using SGEMERJWEB.Bll.Relatorios;
using SGEMERJWEB.WebAPI.Model;
using SGEMERJWEB.WebAPI.Util;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SGEMERJWEB.Entidade.Relatorios;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SGEMERJWEB.WebAPI.Controllers.Relatorios
{
    [System.Web.Http.Authorize]
    public class RelFuncionariosAtivosController : ApiController
    {
        private BllRelFuncionariosAtivos bllRelFuncionariosAtivos;

        [System.Web.Http.Route("api/relfuncionariosativos")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult RetornaFuncionariosAtivos(int tipo = 0, string dataInicio = "", string dataFim = "")
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                bllRelFuncionariosAtivos = new BllRelFuncionariosAtivos(null);

                var listaFuncionariosAtivos = bllRelFuncionariosAtivos.RetornarFuncionariosAtivos(tipo, dataInicio, dataFim);

                if (listaFuncionariosAtivos != null)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, listaFuncionariosAtivos, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(1, 1, 1, listaFuncionariosAtivos, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }

        [System.Web.Http.Route("api/relfuncionariosativos/ExportarPdfExcel")]
        [System.Web.Http.HttpPost]
        public ActionResult ExportarPdfExcel([FromBody] FileBaseParameters parameters)
        {
            try
            {
                bllRelFuncionariosAtivos = new BllRelFuncionariosAtivos(null);
                Uteis uteis = new Uteis();
                string relatorio = string.Empty;

                string nomeRelatorio = "ListaFuncionariosAtivos_" + DateTime.Now.ToShortDateString().Replace("/", "") + DateTime.Now.ToLongTimeString().Replace(":", "");

                List<FuncionariosAtivos> htmlContent2 = (List<FuncionariosAtivos>)bllRelFuncionariosAtivos.RetornarFuncionariosAtivos(parameters.tipo, parameters.dataInicio, parameters.dataFim);

                if (parameters.tipoArquivo == "xls")
                {
                    relatorio = uteis.gerarArquivoPDF(nomeRelatorio, "relFuncionariosAtivos", htmlContent2, null, "xls");
                }
                else
                {
                    relatorio = uteis.gerarArquivoPDF(nomeRelatorio, "relFuncionariosAtivos", htmlContent2, null, "pdf");
                }

                var dir = HttpContext.Current.Server.MapPath(@"~/temp/") + relatorio;

                byte[] fileBytes = File.ReadAllBytes(dir);

                return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet);             
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);             
            }
        }
    }
}