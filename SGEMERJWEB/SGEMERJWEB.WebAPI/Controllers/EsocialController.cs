using Newtonsoft.Json;
using SGEMERJWEB.Bll;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.WebAPI.Model;
using SGEMERJWEB.WebAPI.Util;
using SGEMERJWEB.WebAPI.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Security.Claims;
using App = System.Configuration.ConfigurationManager;

namespace SGEMERJWEB.WebAPI.Controllers
{
    [Authorize]
    public class EsocialController : ApiController
    {
        private BllEsocial _bllEsocial;

        [Route("api/esocial")]
        [HttpGet]
        public IHttpActionResult ObterLista(string e, string n = "", string s = "", int p = 1, string cpf ="")
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                _bllEsocial = new BllEsocial(null, null);

                var retorno = _bllEsocial.RetornarEnvioPaginado(e, n, s, p,cpf);
                var lstEnvioEv = retorno.Item4;
                var totalPagina = retorno.Item2;
                var totalRegistro = retorno.Item3;
                var pagina = retorno.Item1;

                if (lstEnvioEv != null && lstEnvioEv.Any())
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(totalPagina, totalRegistro, pagina, lstEnvioEv, Constante.P_STATUS_OK)));
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


        // POST api/values
        [Route("api/esocial/{evento}")]
        [HttpPost]
        public async Task<IHttpActionResult> Enviar(List<int> valor, string evento, string referencia = "")
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;
            List<RetornoMensagem> retorno = new List<RetornoMensagem>();

            try
            {
                _bllEsocial = new BllEsocial(TokenService.GetCodUsu(User.Identity as ClaimsIdentity),
                                             TokenService.GetIdUsu(User.Identity as ClaimsIdentity));

                string id = String.Join(",", valor.ToArray());
                switch (evento.ToUpper()) {
                    case "S-2300":
                        retorno = await EnviarEventoAsync2300(evento, referencia, id);
                        break;
                    case "S-2306":
                        retorno = await EnviarEventoAsync2306(id, evento, referencia);
                        break;
                    case "S-2205":
                        retorno = await EnviarEventoAsync2205(id, evento, referencia);
                        break;
                    case "S-2399":
                        retorno = await EnviarEventoAsync2399(id, evento, referencia);
                        break;
                }

                if (retorno.Count > 0 ){
                    var eOk = retorno[0].sucesso ? Constante.P_STATUS_OK : Constante.P_STATUS_NOK;
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, null , eOk, retorno[0].mensagem)));
                }
                else{
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Ocorreu um erro ao enviar!"));
                }
            }catch (Exception ex){
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString()));
            }
            return responseMessageResult;
        }

        private async Task<List<RetornoMensagem>> EnviarEventoAsync2300(string evento , string referencia , string valor )
        {
            string ambiente = App.AppSettings["AmbienteApp"].ToString();
            string requestUri = App.AppSettings["IntegracaoEsocial." + ambiente].ToString();
            string userName = App.AppSettings["MpsHeaderUserName." + ambiente].ToString();
            List<RetornoMensagem> retorno;

            try
            {
                List<EventoEsocial> lstEvSocial = new List<EventoEsocial>();
                lstEvSocial = _bllEsocial.RetornarEventoS2300(evento, referencia, valor).ToList();
                requestUri += string.Format(@"{0}/{1}/{2}/{3}",
                                            lstEvSocial[0].tabela,
                                            lstEvSocial[0].tipoInscricao,
                                            lstEvSocial[0].nroInscricao,
                                            lstEvSocial[0].anoMesReferencia);

                using (var Content = new StringContent(JsonConvert.SerializeObject(lstEvSocial), System.Text.Encoding.UTF8, "application/json"))
                {
                    using (var client = new HttpClient())
                    {
                        //Clear request Headers
                        client.DefaultRequestHeaders.Clear();

                        //Define request data format
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("username", userName);

                        var response = await client.PutAsync(requestUri, Content);

                        //iremos armazenar json que foi enviado para  mps 
                        var jsonEnvio = JsonConvert.SerializeObject(lstEvSocial); //Variavel para guardar o json que foi enviado a mps
                        _bllEsocial.gravarEnvioJsonMPS(jsonEnvio, evento, lstEvSocial[0].anoMesReferencia);

                        //iremos armazenar json de retorno da mps 
                        var jsonRetorno = response.Content.ReadAsStringAsync().Result; //variavel para guarda o json retornado pela mps
                        retorno = _bllEsocial.gravarRetorno(JsonConvert.DeserializeObject<IEnumerable<EsocialRelatorio>>(jsonRetorno));
                    }
                }
            }
            catch (System.IO.IOException ioEx)
            {
                throw new Exception(ioEx.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return retorno;
        }

        private async Task<List<RetornoMensagem>> EnviarEventoAsync2306(string valor, string evento = "S-2306", string referencia="")
        {
            string ambiente = App.AppSettings["AmbienteApp"].ToString();
            string requestUri = App.AppSettings["IntegracaoEsocial." + ambiente].ToString();
            string userName = App.AppSettings["MpsHeaderUserName." + ambiente].ToString();
            List<RetornoMensagem> retorno;

            try
            {
                List<EventoEsocial2306> lstEvSocial2306 = new List<EventoEsocial2306>();
                lstEvSocial2306 = _bllEsocial.RetornarEventoS2306(evento, referencia, valor).ToList();
                requestUri += string.Format(@"{0}/{1}/{2}/{3}",
                                            lstEvSocial2306[0].tabela,
                                            lstEvSocial2306[0].tipoInscricao,
                                            lstEvSocial2306[0].nroInscricao,
                                            lstEvSocial2306[0].anoMesReferencia);

                using (var Content = new StringContent(JsonConvert.SerializeObject(lstEvSocial2306), System.Text.Encoding.UTF8, "application/json"))
                {
                    using (var client = new HttpClient())
                    {
                        //Clear request Headers
                        client.DefaultRequestHeaders.Clear();

                        //Define request data format
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("username", userName);

                        var response = await client.PutAsync(requestUri, Content);

                        //iremos armazenar json que foi enviado para  mps 
                        var jsonEnvio = JsonConvert.SerializeObject(lstEvSocial2306); //Variavel para guardar o json que foi enviado a mps
                        _bllEsocial.gravarEnvioJsonMPS(jsonEnvio, evento, lstEvSocial2306[0].anoMesReferencia);

                        //iremos armazenar json de retorno da mps 
                        var jsonRetorno = response.Content.ReadAsStringAsync().Result; //variavel para guarda o json retornado pela mps
                        retorno = _bllEsocial.gravarRetorno(JsonConvert.DeserializeObject<IEnumerable<EsocialRelatorio>>(jsonRetorno));
                    }
                }
            }
            catch (System.IO.IOException ioEx)
            {
                throw new Exception(ioEx.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return retorno;
        }

        private async Task<List<RetornoMensagem>> EnviarEventoAsync2205(string valor, string evento = "S-2205", string referencia = "")
        {
            string ambiente = App.AppSettings["AmbienteApp"].ToString();
            string requestUri = App.AppSettings["IntegracaoEsocial." + ambiente].ToString();
            string userName = App.AppSettings["MpsHeaderUserName." + ambiente].ToString();

            List<RetornoMensagem> retorno;

            try
            {
                List<EventoEsocial2205> lstEvSocial2205 = new List<EventoEsocial2205>();
                lstEvSocial2205 = _bllEsocial.RetornarEventoS2205(evento, referencia, valor).ToList();
                requestUri += string.Format(@"{0}/{1}/{2}/{3}",
                                            lstEvSocial2205[0].tabela,
                                            lstEvSocial2205[0].tipoInscricao,
                                            lstEvSocial2205[0].nroInscricao,
                                            lstEvSocial2205[0].anoMesReferencia);

                using (var Content = new StringContent(JsonConvert.SerializeObject(lstEvSocial2205), System.Text.Encoding.UTF8, "application/json"))
                {
                    using (var client = new HttpClient())
                    {
                        //Clear request Headers
                        client.DefaultRequestHeaders.Clear();

                        //Define request data format
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("username", userName);

                        var response = await client.PutAsync(requestUri, Content);

                        //iremos armazenar json que foi enviado para  mps 
                        var jsonEnvio = JsonConvert.SerializeObject(lstEvSocial2205); //Variavel para guardar o json que foi enviado a mps
                        _bllEsocial.gravarEnvioJsonMPS(jsonEnvio, evento, lstEvSocial2205[0].anoMesReferencia);

                        //iremos armazenar json de retorno da mps 
                        var jsonRetorno = response.Content.ReadAsStringAsync().Result; //variavel para guarda o json retornado pela mps
                        retorno = _bllEsocial.gravarRetorno(JsonConvert.DeserializeObject<IEnumerable<EsocialRelatorio>>(jsonRetorno));
                    }
                }
            }
            catch (System.IO.IOException ioEx)
            {
                throw new Exception(ioEx.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return retorno;
        }


        private async Task<List<RetornoMensagem>> EnviarEventoAsync2399(string valor, string evento = "S-2399", string referencia = "")
        {
            string ambiente = App.AppSettings["AmbienteApp"].ToString();
            string requestUri = App.AppSettings["IntegracaoEsocial." + ambiente].ToString();
            string userName = App.AppSettings["MpsHeaderUserName." + ambiente].ToString();

            List<RetornoMensagem> retorno;

            try
            {
                List<EventoEsocial2399> lstEvSocial2399 = new List<EventoEsocial2399>();
                lstEvSocial2399 = _bllEsocial.RetornarEventoS2399(evento, referencia, valor).ToList();
                requestUri += string.Format(@"{0}/{1}/{2}/{3}",
                                            lstEvSocial2399[0].tabela,
                                            lstEvSocial2399[0].tipoInscricao,
                                            lstEvSocial2399[0].nroInscricao,
                                            lstEvSocial2399[0].anoMesReferencia);

                using (var Content = new StringContent(JsonConvert.SerializeObject(lstEvSocial2399), System.Text.Encoding.UTF8, "application/json"))
                {
                    using (var client = new HttpClient())
                    {
                        //Clear request Headers
                        client.DefaultRequestHeaders.Clear();

                        //Define request data format
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("username", userName);

                        var response = await client.PutAsync(requestUri, Content);

                        //iremos armazenar json que foi enviado para  mps 
                        var jsonEnvio = JsonConvert.SerializeObject(lstEvSocial2399); //Variavel para guardar o json que foi enviado a mps
                        _bllEsocial.gravarEnvioJsonMPS(jsonEnvio, evento, lstEvSocial2399[0].anoMesReferencia);

                        //iremos armazenar json de retorno da mps 
                        var jsonRetorno = response.Content.ReadAsStringAsync().Result; //variavel para guarda o json retornado pela mps
                        retorno = _bllEsocial.gravarRetorno(JsonConvert.DeserializeObject<IEnumerable<EsocialRelatorio>>(jsonRetorno));
                    }
                }
            }
            catch (System.IO.IOException ioEx)
            {
                throw new Exception(ioEx.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return retorno;
        }

        [Route("api/esocial/inconsistencia")]
        [HttpGet]
        public IHttpActionResult ComunicadorInconsistencia(string idComunicador)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                _bllEsocial = new BllEsocial(null,
                                             null);

                var retorno = _bllEsocial.comunicadorInconsistencia(idComunicador);

                if (retorno.Count > 0)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, retorno, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, retorno, Constante.P_STATUS_NOK)));
                }
            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }


        [Route("api/esocial/originalEnviado/{colaborador}/{tipoColaborador}")]
        [HttpGet]
        public IHttpActionResult eventoOriginalEnviadoS2300(int colaborador, int tipoColaborador)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                _bllEsocial = new BllEsocial(null, null);

                var retorno = _bllEsocial.eventoOriginalEnviadoS2300(colaborador, tipoColaborador);

                if (retorno == true)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, retorno, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, retorno, Constante.P_STATUS_NOK)));
                }
            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }

        private async Task<string> importarDadosEsocial(string evento, string referencia, bool import)
        {
            string ambiente = App.AppSettings["AmbienteApp"].ToString();
            string requestUri = App.AppSettings["IntegracaoRelatorioEsocial." + ambiente].ToString();
            string requestUriErros = App.AppSettings["IntegracaoRelatorioEsocialErros." + ambiente].ToString();
            string CNPJ = App.AppSettings["IntegracaoRelatorioEsocialCNPJ." + ambiente].ToString();
            string retorno = "" ;
            string jsonRetorno = "";
            string jsonRetornoErros = "";

            try
            {
                if (import == true)
                {
                    requestUri += string.Format(@"{0}/{1}/{2}/{3}?{4}", "1", CNPJ, referencia, evento, "fase=ENVIO");
                    requestUriErros += string.Format(@"{0}/{1}/{2}/{3}?{4}", "1", CNPJ, referencia, evento, "fase=ENVIO");
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        var response = await client.GetAsync(requestUri);
                        var responseErros = await client.GetAsync(requestUriErros);
                        if (response.IsSuccessStatusCode == true)
                        {
                            jsonRetorno = response.Content.ReadAsStringAsync().Result;
                            jsonRetornoErros = responseErros.Content.ReadAsStringAsync().Result;
                            EsocialRelatorioDados dados = JsonConvert.DeserializeObject<EsocialRelatorioDados>(jsonRetorno);
                            EsocialRelatorioDadosErros dadosErros = JsonConvert.DeserializeObject<EsocialRelatorioDadosErros>(jsonRetornoErros);
                            dados.Evento = evento;
                            retorno =  _bllEsocial.importarRelatorioDados(dados);
                            _bllEsocial.importaErrosEsocial(dadosErros);
                        }
                        else{
                            retorno = "NAO_IMPORTADO";
                        }
                    }
                }else{
                    retorno = "NAO_IMPORTADO";
                }
            }
            catch (System.IO.IOException ioEx)
            {
                throw new Exception(ioEx.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return retorno;
        }

        [Route("api/esocial/relatorio")]
        [HttpGet]
        public async Task<IHttpActionResult> relatorioDadosEsocial(string evento = "", 
                                                                   string referencia = "", 
                                                                   int? status = 0,
                                                                   string cpf = "",
                                                                   string nome = "",
                                                                   bool import = false, 
                                                                   int pagina = 1 )
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                _bllEsocial = new BllEsocial(null, null);

                var retornoImport = await importarDadosEsocial(evento, referencia, import);

                var retorno = _bllEsocial.RelatorioDadosEsocialPaginado(evento, referencia, status, cpf, nome, pagina);
                var lstRelatorioDadosEsocial = retorno.Item4;
                var totalPagina = retorno.Item2;
                var totalRegistro = retorno.Item3;
                var pag = retorno.Item1;

                if (lstRelatorioDadosEsocial != null && lstRelatorioDadosEsocial.Any())
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, 
                                                                                    RetornoModel.CriarRetorno(totalPagina, 
                                                                                    totalRegistro, 
                                                                                    pag,
                                                                                    lstRelatorioDadosEsocial, 
                                                                                    Constante.P_STATUS_OK,
                                                                                    retornoImport)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, 
                                                                                    RetornoModel.CriarRetorno(totalPagina, 
                                                                                    totalRegistro, 
                                                                                    pagina, 
                                                                                    null, 
                                                                                    Constante.P_STATUS_NOK,
                                                                                    Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }
            }
            catch (Exception ex)
            {
                responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError,
                                                                               RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }

            return responseMessageResult;
        }


        [Route("api/esocial/retornoErros/{identificador}")]
        [HttpGet]
        public IHttpActionResult consultaRetornoErrosEsocial(string identificador)
        {
            System.Web.Http.Results.ResponseMessageResult responseMessageResult;

            try
            {
                _bllEsocial = new BllEsocial(null, null);

                var retorno = _bllEsocial.consultaRetornoErrosEsocial(identificador);

                if (retorno.Count > 0)
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, retorno, Constante.P_STATUS_OK)));
                }
                else
                {
                    responseMessageResult = ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(null, null, null, retorno, Constante.P_STATUS_NOK)));
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
