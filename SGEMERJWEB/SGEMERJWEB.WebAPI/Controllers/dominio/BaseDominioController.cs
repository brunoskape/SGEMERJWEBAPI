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
using System.Web.Http.Results;

namespace SGEMERJWEB.WebAPI.Controllers.Dominio
{
    //[Authorize]    
    public class BasaDominioController : ApiController
    {
        
        [Route("api/paises")]
        [HttpGet]
        public IHttpActionResult ObterListaPaises()
        {
           
            var objeto = BllDominioFachada.BllPais(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/paises/{id}")]
        [HttpGet]
        public IHttpActionResult ObterPais(int id)
        {

            var objeto = BllDominioFachada.BllPais(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/classificacaodeficiencia")]
        [HttpGet]
        public IHttpActionResult ObterListaClassificacaoDeficiencia()
        {

            var objeto = BllDominioFachada.BllClassificacaoDeficiencia(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/classificacaodeficiencia/{id}")]
        [HttpGet]
        public IHttpActionResult ObterClassificacaoDeficiencia(int id)
        {

            var objeto = BllDominioFachada.BllClassificacaoDeficiencia(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/estadocivil")]
        [HttpGet]
        public IHttpActionResult ObterListaEstadoCivil()
        {

            var objeto = BllDominioFachada.BllEstadoCivil(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/estadocivil/{id}")]
        [HttpGet]
        public IHttpActionResult ObterEstadoCivil(int id)
        {

            var objeto = BllDominioFachada.BllEstadoCivil(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/grauinstrucao")]
        [HttpGet]
        public IHttpActionResult ObterListaGrauInstrucao()
        {

            var objeto = BllDominioFachada.BllGrauInstrucao(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/grauinstrucao/{id}")]
        [HttpGet]
        public IHttpActionResult ObterGrauInstrucao(int id)
        {

            var objeto = BllDominioFachada.BllGrauInstrucao(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/racas")]
        [HttpGet]
        public IHttpActionResult ObterListaRaca()
        {

            var objeto = BllDominioFachada.BllRaca(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/racas/{id}")]
        [HttpGet]
        public IHttpActionResult ObterRaca(int id)
        {

            var objeto = BllDominioFachada.BllRaca(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/uf")]
        [HttpGet]
        public IHttpActionResult ObterListaUf()
        {

            var objeto = BllDominioFachada.BllUnidadeFederacao(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/uf/{uf}")]
        [HttpGet]
        public IHttpActionResult ObterUf(string uf)
        {

            var objeto = BllDominioFachada.BllUnidadeFederacao(GetCodUsu).Obter(uf);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/tipocolaborador")]
        [HttpGet]
        public IHttpActionResult ObterListaTipoColaborador()
        {

            var objeto = BllDominioFachada.BllTipoColaborador(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/tipocolaborador/{id}")]
        [HttpGet]
        public IHttpActionResult ObterTipoColaborador(int id)
        {

            var objeto = BllDominioFachada.BllTipoColaborador(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/tipologradouro")]
        [HttpGet]
        public IHttpActionResult ObterListaTipoLogradouro()
        {

            var objeto = BllDominioFachada.BllTipoLogradouro(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/tipologradouro/{id}")]
        [HttpGet]
        public IHttpActionResult ObterTipoLogradouro(int id)
        {

            var objeto = BllDominioFachada.BllTipoLogradouro(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/unidadepagamento")]
        [HttpGet]
        public IHttpActionResult ObterListaUnidadePagamento()
        {

            var objeto = BllDominioFachada.BllUnidadePagamento(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/unidadepagamento/{id}")]
        [HttpGet]
        public IHttpActionResult ObterUnidadePagamento(int id)
        {

            var objeto = BllDominioFachada.BllUnidadePagamento(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/temporesidencia")]
        [HttpGet]
        public IHttpActionResult ObterListaTempoResidencia()
        {

            var objeto = BllDominioFachada.BllTempoResidenciaEstrangeiro(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/temporesidencia/{id}")]
        [HttpGet]
        public IHttpActionResult ObterTempoResidencia(int id)
        {

            var objeto = BllDominioFachada.BllTempoResidenciaEstrangeiro(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/classificacaoestrangeiro")]
        [HttpGet]
        public IHttpActionResult ObterListaClassificacaoTrabEstrangeiro()
        {

            var objeto = BllDominioFachada.BllClassificacaoTrabEstrangeiro(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/classificacaoestrangeiro/{id}")]
        [HttpGet]
        public IHttpActionResult ObterClassificacaoTrabEstrangeiro(int id)
        {

            var objeto = BllDominioFachada.BllClassificacaoTrabEstrangeiro(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/tipodependente")]
        [HttpGet]
        public IHttpActionResult ObterListaTipoDependente()
        {

            var objeto = BllDominioFachada.BllTipoDependente(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/classificacaoestrangeiro/{id}")]
        [HttpGet]
        public IHttpActionResult ObterTipoDependente(int id)
        {

            var objeto = BllDominioFachada.BllTipoDependente(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/motivoafastamento")]
        [HttpGet]
        public IHttpActionResult ObterListaMotivoAfastamento()
        {

            var objeto = BllDominioFachada.BllMotivoAfastamento(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/motivoafastamento/{id}")]
        [HttpGet]
        public IHttpActionResult ObterMotivoAfastamento(int id)
        {

            var objeto = BllDominioFachada.BllMotivoAfastamento(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/naturezaestagio")]
        [HttpGet]
        public IHttpActionResult ObterListaNaturezaEstagio()
        {

            var objeto = BllDominioFachada.BllNaturezaEstagio(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/naturezaestagio/{id}")]
        [HttpGet]
        public IHttpActionResult ObterNaturezaEstagio(int id)
        {

            var objeto = BllDominioFachada.BllNaturezaEstagio(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/cbocargo")]
        [HttpGet]
        public IHttpActionResult ObterListaCBOCargo()
        {

            var objeto = BllDominioFachada.BllCBOCargo(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/cbocargo/{id}")]
        [HttpGet]
        public IHttpActionResult ObterCBOCargo(int id)
        {

            var objeto = BllDominioFachada.BllCBOCargo(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/nivelestagio")]
        [HttpGet]
        public IHttpActionResult ObterListaNivelEstagio()
        {

            var objeto = BllDominioFachada.BllNivelEstagio(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/nivelestagio/{id}")]
        [HttpGet]
        public IHttpActionResult ObterNivelEstagio(int id)
        {

            var objeto = BllDominioFachada.BllNivelEstagio(GetCodUsu).Obter(id);
            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/setores/combo/ativos")]
        [HttpGet]
        public IHttpActionResult ObterListaSetoresAtivos()
        {

            var objetoBLL = new BllSetor(GetCodUsu);

            var objeto = objetoBLL.RetornarSetoresAtivos();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/cadastramentoinicial")]
        [HttpGet]
        public IHttpActionResult ObterListaCadastramentoInicial()
        {

            var objeto = BllDominioFachada.BllCadastramentoInicial(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/cadastramentoinicial/{id}")]
        [HttpGet]
        public IHttpActionResult ObterCadastramentoInicial(int id)
        {

            var objeto = BllDominioFachada.BllCadastramentoInicial(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }



        [Route("api/naturezaatividade")]
        [HttpGet]
        public IHttpActionResult ObterListaNaturezaAtividade()
        {

            var objeto = BllDominioFachada.BllNaturezaAtividade(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }


        [Route("api/naturezaatividade/{id}")]
        [HttpGet]
        public IHttpActionResult ObterNaturezaAtividade(int id)
        {

            var objeto = BllDominioFachada.BllNaturezaAtividade(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/categoriatrabalhador")]
        [HttpGet]
        public IHttpActionResult ObterListaCategoriaTrabalhador()
        {

            var objeto = BllDominioFachada.BllCategoriaTrabalhador(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);

        }

        [Route("api/categoriatrabalhador/{id}")]
        [HttpGet]
        public IHttpActionResult ObterCategoriaTrabalhador(int id)
        {
            var objeto = BllDominioFachada.BllCategoriaTrabalhador(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/esocialevento")]
        [HttpGet]
        public IHttpActionResult ObterListaEsocialEvento()
        {

            var objeto = BllDominioFachada.BllEsocialEvento(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);

        }

        [Route("api/esocialevento/{id}")]
        [HttpGet]
        public IHttpActionResult ObterListaEsocialEvento(int id)
        {
            var objeto = BllDominioFachada.BllEsocialEvento(GetCodUsu).Obter(id);

            return TratarObjetoParaResposta(objeto);
        }

        private ResponseMessageResult TratarObjetoParaResposta(object objeto)
        {
            try
            {
                if (objeto != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(0, 0, 0, objeto, Constante.P_STATUS_OK)));
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, RetornoModel.CriarRetorno(0, 0, 0, objeto, Constante.P_STATUS_NOK, Constante.M_REGISTRO_NAO_ENCONTRADO)));
                }

            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, RetornoModel.CriarRetorno(null, null, null, null, Constante.P_STATUS_NOK, ex.Message.ToString())));
            }
        }

        private string GetCodUsu
        {
            get
            {
                return TokenService.GetCodUsu(User.Identity as ClaimsIdentity);
            }
        }

        [Route("api/motivorescisao")]
        [HttpGet]
        public IHttpActionResult ObterMotivoRescisao()
        {

            var objeto = BllDominioFachada.BllMotivoRescisao(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);

        }

        [Route("api/recibosEsocial/{id}")]
        [HttpGet]
        public IHttpActionResult ObterRecibosEsocial(int id)
        {

            var objeto = BllDominioFachada.BllRecibosEsocial(GetCodUsu).ObterLista(id);

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/tipobeneficio/combo/ativos")]
        [HttpGet]
        public IHttpActionResult ObterListaTipoBeneficioAtivos()
        {

            var objetoBLL = new BllTipoBeneficio(GetCodUsu);

            var objeto = objetoBLL.RetornarTipoBeneficioesAtivos();

            return TratarObjetoParaResposta(objeto);
        }

        [Route("api/situacao")]
        [HttpGet]
        public IHttpActionResult ObterListaSituacao()
        {

            var objeto = BllDominioFachada.BllSituacao(GetCodUsu).ObterLista();

            return TratarObjetoParaResposta(objeto);
        }

    }
}

