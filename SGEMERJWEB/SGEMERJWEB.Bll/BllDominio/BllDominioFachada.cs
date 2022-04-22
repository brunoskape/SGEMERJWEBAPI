using SGEMERJWEB.Entidade.Dominio;

namespace SGEMERJWEB.Bll
{
    public abstract class BllDominioFachada
    {
        public static BllDominioBase<ClassificacaoDeficiencia> BllClassificacaoDeficiencia(string codusu = null)
        {


            var _bllClassificacaoDeficiencia = new BllDominioBase<ClassificacaoDeficiencia>(codusu, "PKG_COMBOS.SP_DEFICIENCIA");
            return _bllClassificacaoDeficiencia;

        }

        public static BllUnidadeFederacao BllUnidadeFederacao(string codusu = null)
        {

            var _bllUnidadeFederacao = new BllUnidadeFederacao(codusu);
            return _bllUnidadeFederacao;

        }

        public static BllNaturezaEstagio BllNaturezaEstagio(string codusu = null)
        {

            var _bllNaturezaEstagio = new BllNaturezaEstagio(codusu);
            return _bllNaturezaEstagio;

        }

        public static BllCBOCargo BllCBOCargo(string codusu = null)
        {

            var _bllCBOCargo = new BllCBOCargo(codusu);
            return _bllCBOCargo;

        }

        public static BllDominioBase<EstadoCivil> BllEstadoCivil(string codusu = null)
        {

            var _bllEstadoCivil = new BllDominioBase<EstadoCivil>(codusu, "PKG_COMBOS.SP_ESTADOCIVIL");
            return _bllEstadoCivil;

        }

        public static BllDominioBase<GrauInstrucao> BllGrauInstrucao(string codusu = null)
        {

            var _bllGrauInstrucao = new BllDominioBase<GrauInstrucao>(codusu, "PKG_COMBOS.SP_GRAUINSTRUCAO");
            return _bllGrauInstrucao;

        }

        public static BllDominioBase<Pais> BllPais(string codusu = null)
        {

            var _bllPais = new BllDominioBase<Pais>(codusu, "PKG_COMBOS.SP_PAIS");
            return _bllPais;

        }

        public static BllDominioBase<Raca> BllRaca(string codusu = null)
        {

            var _bllRaca = new BllDominioBase<Raca>(codusu, "PKG_COMBOS.SP_RACA");
            return _bllRaca;

        }

        public static BllDominioBase<TipoColaborador> BllTipoColaborador(string codusu = null)
        {

            var _bllTipoColaborador = new BllDominioBase<TipoColaborador>(codusu, "PKG_COMBOS.SP_TIPOCOLABORADOR");
            return _bllTipoColaborador;

        }

        public static BllDominioBase<TipoLogradouro> BllTipoLogradouro(string codusu = null)
        {

            var _bllTipoLogradouro = new BllDominioBase<TipoLogradouro>(codusu, "PKG_COMBOS.SP_TIPOLOGRADOURO");
            return _bllTipoLogradouro;

        }

        public static BllDominioBase<UnidadePagamento> BllUnidadePagamento(string codusu = null)
        {

            var _bllUnidadePagamento = new BllDominioBase<UnidadePagamento>(codusu, "PKG_COMBOS.SP_UNIDADEPAGAMENTO");
            return _bllUnidadePagamento;

        }
        public static BllDominioBase<ClassificacaoTrabEstrangeiro> BllClassificacaoTrabEstrangeiro(string codusu = null)
        {

            var _bllClassificacaoTrabEstrangeiro = new BllDominioBase<ClassificacaoTrabEstrangeiro>(codusu, "PKG_COMBOS.SP_CLASSIFTRABESTRANGEIRO");
            return _bllClassificacaoTrabEstrangeiro;

        }

        public static BllDominioBase<TempoResidenciaEstrangeiro> BllTempoResidenciaEstrangeiro(string codusu = null)
        {

            var _bllTempoResidenciaEstrangeiro = new BllDominioBase<TempoResidenciaEstrangeiro>(codusu, "PKG_COMBOS.SP_TEMPORESIDENCIAESTRANGEIRO");
            return _bllTempoResidenciaEstrangeiro;

        }

        public static BllDominioBase<TipoDependente> BllTipoDependente(string codusu = null)
        {

            var _bllTipoDependente = new BllDominioBase<TipoDependente>(codusu, "PKG_COMBOS.SP_TIPODEPENDENTE");
            return _bllTipoDependente;

        }

        public static BllDominioBase<MotivoAfastamento> BllMotivoAfastamento(string codusu = null)
        {

            var _bllMotivoAfastamento = new BllDominioBase<MotivoAfastamento>(codusu, "PKG_COMBOS.SP_MOTIVOAFASTAMENTO");
            return _bllMotivoAfastamento;

        }

        public static BllDominioBase<NivelEstagio> BllNivelEstagio(string codusu = null)
        {

            var _bllNivelEstagio = new BllDominioBase<NivelEstagio>(codusu, "PKG_COMBOS.SP_NIVELESTAGIO");
            return _bllNivelEstagio;
        }

        public static BllDominioBase<CadastramentoInicial> BllCadastramentoInicial(string codusu = null)
        {

            var _bllCadastramentoInicial = new BllDominioBase<CadastramentoInicial>(codusu, "PKG_COMBOS.SP_CADASTRAMENTOINICIAL");
            return _bllCadastramentoInicial;

        }

        public static BllDominioBase<NaturezaAtividade> BllNaturezaAtividade(string codusu = null)
        {

            var _bllNaturezaAtividade = new BllDominioBase<NaturezaAtividade>(codusu, "PKG_COMBOS.SP_NATUREZAATIVIDADE");
            return _bllNaturezaAtividade;

        }

        public static BllDominioBase<CategoriaTrabalhador> BllCategoriaTrabalhador(string codusu = null)
        {

            var _bllCategoriaTrabalhador = new BllDominioBase<CategoriaTrabalhador>(codusu, "PKG_COMBOS.SP_CATEGORIATRABALHADOR");
            return _bllCategoriaTrabalhador;

        }

        public static BllDominioBase<EsocialEvento> BllEsocialEvento(string codusu = null)
        {

            var _bllEsocialEvento = new BllDominioBase<EsocialEvento>(codusu, "PKG_COMBOS.CNS_ESOCIAL_EVENTO");
            return _bllEsocialEvento;

        }

        public static BllDominioBase<MotivoRescisao> BllMotivoRescisao(string codusu = null)
        {
            return new BllDominioBase<MotivoRescisao>(codusu, "PKG_COMBOS.cns_motivo_recisao");

        }

        public static BllRecibosEsocial BllRecibosEsocial(string codusu = null)
        {

            var _bllRecibosEsocial = new BllRecibosEsocial(codusu);
            return _bllRecibosEsocial;
        }

        public static BllDominioBase<Situacao> BllSituacao(string codusu = null)
        {

            var _bllSituacao = new BllDominioBase<Situacao>(codusu, "PKG_COMBOS.SP_SITUACAO");
            return _bllSituacao;

        }
    }

}
