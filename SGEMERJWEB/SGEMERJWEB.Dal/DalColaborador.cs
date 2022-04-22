using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.Entidade.DTO;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;

namespace SGEMERJWEB.Dal
{
    public class DalColaborador : DalBase
    {
        private readonly string codUsuario;
        private readonly string idUsuario;

        public DalColaborador(string codusu) : base(codusu)
        {
            codUsuario = codusu;
        }

        public DalColaborador(string codusu, string idusu) : base(codusu)
        {
            codUsuario = codusu;
            idUsuario = idusu;
        }

        public string Gravar(Colaborador colaborador)
        {
            var ret = "";

            try
            {
                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }

                trans = sd.BeginTransaction();

                if (colaborador.Id <= 0)
                {
                    //não existe nada do colaborador
                    Int32 idColaborador = IncluirDadoGeral(colaborador);

                    colaborador.Id = idColaborador;

                    colaborador.IdDoTipo = IncluirTipo(colaborador);

                    IncluirListasDoColaborador(colaborador);

                    IncluirComplementosDoColaborador(colaborador);

                    ret = "INCLUIDO";
                }
                else
                {
                    DalEsocial _dalEsocial = new DalEsocial(codUsuario,idUsuario);

                    if(_dalEsocial.eventoOriginalEnviadoS2300(colaborador.Id, colaborador.TipoColaborador.Id)) {

                        //Incluir na tabela de esocial comunicado, somente se for tipo estágiario. 
                        if (colaborador.TipoColaborador.Id == 1)
                        {
                            DalEsocial objDalEsocial = new DalEsocial(codUsuario, idUsuario);
                            //incluir o protocolo para um evento esocial S-2300, quando informado o protocolo significa que está retificando um dado que
                            //foi cadastrado de forma errada.
                            if (string.IsNullOrEmpty(colaborador.Protocolo) == false) //aqui foi informado o protocolo
                            {
                                string[] dadosRetificadora = Array.ConvertAll(colaborador.Protocolo.Split('|'), p => p.Trim());


                                objDalEsocial.gravarRetificadora(colaborador.Id,
                                                                      colaborador.TipoColaborador.Id,
                                                                      dadosRetificadora[0],
                                                                      dadosRetificadora[1]);
                            }
                            else
                            {
                                //eventos S-2306 e/ou S-2205 do esocial.
                                objDalEsocial.gravarEsocialComunicadorAlteracao(colaborador);
                            }
                        }
                    }

                    AlterarDadoGeral(colaborador);//alteração dos dados básicos

                    if (colaborador.Id > 0 && colaborador.IdDoTipo <= 0)
                    {
                        //colaborador existe mas não existe o tipo
                        colaborador.IdDoTipo = IncluirTipo(colaborador);

                        ret = "INCLUIDO-TIPO";
                    }
                    else if (colaborador.Id > 0 && colaborador.IdDoTipo > 0 && string.IsNullOrEmpty(ret))
                    {
                        //colaborador existe e existe o tipo
                        colaborador.IdDoTipo = AlterarTipo(colaborador);

                        //inclusão de listas é realizada com exclusão dos dados do bd para incluir novamente com novos objetos.
                        RemoverListasDoColaborador(colaborador.IdDoTipo);

                        //inclusão de dados complementares (remuneracao, imigrante) é realizada com exclusão dos dados do bd para incluir novamente com novos objetos.
                        RemoverOutrosDadosDoColaborador(colaborador.IdDoTipo);

                        ret = "ALTERADO";
                    }

                    IncluirListasDoColaborador(colaborador);

                    IncluirComplementosDoColaborador(colaborador);

                }

                trans.Commit();
            }
            catch (ServicoDadosODPNETM.ServicoDadosException sdEx)
            {
                ret = "ERRO";
                trans.Rollback();
                throw new Exception(sdEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                ret = "ERRO";
                trans.Rollback();
                throw ex;
            }
            finally
            {
                sd.Connection.Close();
            }

            return ret;
        }

        private void IncluirListasDoColaborador(Colaborador colaborador)
        {
            IncluirEndereco(colaborador);

            IncluirDeficiencia(colaborador);

            IncluirEmail(colaborador);

            IncluirTelefone(colaborador);

            IncluirDependente(colaborador);

            IncluirAfastamento(colaborador);

            IncluirDadosProfissionais(colaborador);

            IncluirCursoExtraCurricular(colaborador);

            IncluirInformacoesAcademica(colaborador);

        }

        private void IncluirComplementosDoColaborador(Colaborador colaborador)
        {
            IncluirRemuneracao(colaborador);

            IncluirImigrante(colaborador);

            IncluirResidenteExterior(colaborador);

            IncluirInformacaoEstagio(colaborador);

            IncluirInstituicaoDeEnsino(colaborador);

            IncluirDadosTSVE(colaborador);

        }

        private void RemoverListasDoColaborador(Int32 idColaboradorTipo)
        {
            sd.ExecutaProc("pkg_colaborador.sp_colaborador_limpa_listas", idColaboradorTipo);
        }

        private void RemoverOutrosDadosDoColaborador(Int32 idColaboradorTipo)
        {
            sd.ExecutaProc("pkg_colaborador.sp_colaborador_limpa_outros", idColaboradorTipo);
        }

        #region Consultas
        public (int totalRegistros, IEnumerable<ColaboradorGridDTO> lista) ObterListaPaginada(string nome, string cpf, int tipo, int pagina, int regPagina)
        {
            var lista = new List<ColaboradorGridDTO>();

            try
            {
                var dtPaginacao = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaboradores_paginado", nome, cpf, tipo, pagina, regPagina, sd.CriaRefCursor()).Tables[0];

                lista = (from DataRow dr in dtPaginacao.Rows
                         select new ColaboradorGridDTO
                         {
                             Id = int.Parse(dr["ID"].ToString()),
                             IdDoTipo = int.Parse(dr["colaborador_tipo"].ToString()),
                             CPF = dr["CPF"].ToString(),
                             Nome = dr["NOME"].ToString(),
                             Ativo = dr["ind_ativo"].ToString() == "S",
                             Tipo = { Id = int.Parse(dr["ID_TIPO"].ToString()), Descricao = dr["DESCRICAO_TIPO"].ToString() },
                             Linha = int.Parse(dr["linha"].ToString()),
                             Rescisao = { Id = int.Parse(dr["idRescisao"].ToString()) }
                         }).ToList();

                int totalRegistros = 0;

                if (dtPaginacao.Rows.Count > 0)
                    totalRegistros = int.Parse(dtPaginacao.Rows[0]["totreg"].ToString());

                return (totalRegistros, lista);
            }
            catch (Exception ex)
            {
                throw new Exception("[ObterListaPaginada] " + ex.ToString());
            }
            finally
            {
                lista = null;
                sd.Connection.Close();
            }
        }

        public Colaborador Obter(int idDoTipo)
        {
            try
            {
                Colaborador colaborador = new Colaborador { Id = 0, IdDoTipo = idDoTipo };

                colaborador = ObterDadosDoColaboradorTipo(colaborador);

                if (colaborador == null)
                    return null;

                colaborador = ObterDadoGeral(colaborador);
                if (colaborador == null)
                    return null;


                colaborador = ObterDeficiencia(colaborador);
                colaborador = ObterEndereco(colaborador);
                colaborador = ObterEmail(colaborador);
                colaborador = ObterTelefone(colaborador);
                colaborador = ObterRemuneracao(colaborador);
                colaborador = ObterResidenteExterior(colaborador);
                colaborador = ObterImigrante(colaborador);


                colaborador = ObterDependente(colaborador);

                colaborador = ObterDadosTSVE(colaborador);

                colaborador = ObterInformacoesAcademica(colaborador);

                //tipo estagiário
                if (colaborador.TipoColaborador.Id == 1)
                {
                    colaborador = ObterInformacaoEstagio(colaborador);
                    colaborador = ObterInstituicaoEnsino(colaborador);
                }

                colaborador = ObterAfastamento(colaborador);
                colaborador = ObterDadosProfissionais(colaborador);
                colaborador = ObterDadosCursoExtra(colaborador);


                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[Obter] " + ex.ToString());
            }

        }

        public Colaborador ObterPorTipoECPF(int tipo, string cpf)
        {
            try
            {
                if (tipo <= 0)
                    return null;

                Colaborador colaborador = null;

                var id = ObterIdPorTipoECPF(tipo, cpf);

                if (id > 0)
                {
                    colaborador = Obter(id);
                }
                else
                {
                    //se o tipo não existe, trago os dados básicos para inclusão do novo tipo
                    colaborador = new Colaborador { CPF = cpf, Id = 0 };
                    colaborador = ObterDadoGeral(colaborador);

                    if (colaborador != null)
                    {
                        colaborador.IdDoTipo = ObterIdDoTipoExistente(colaborador.Id);

                        colaborador = ObterEndereco(colaborador);
                        colaborador = ObterEmail(colaborador);
                        colaborador = ObterTelefone(colaborador);
                        colaborador = ObterImigrante(colaborador);

                        colaborador.IdDoTipo = 0;
                    }
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[ObterPorTipoECPF] " + ex.ToString());
            }

        }

        public int ObterIdDoTipoExistente(int id)
        {
            try
            {
                var dtResultados = sd.ExecutaProcDS("pkg_colaborador.sp_colaborador_id_tipo_existe", id, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {
                    return int.Parse(dtResultados.Rows[0]["id_para_busca"].ToString());
                }
                return 0;

            }
            catch (Exception ex)
            {
                throw new Exception("[CPFJaExiste] " + ex.ToString());
            }
        }

        public bool CPFJaExiste(int id, string cpf)
        {
            try
            {
                var dtResultados = sd.ExecutaProcDS("pkg_colaborador.sp_colaborador_cpf_existe", id, cpf, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {
                    return int.Parse(dtResultados.Rows[0]["existe"].ToString()) > 0;
                }
                return false;

            }
            catch (Exception ex)
            {
                throw new Exception("[CPFJaExiste] " + ex.ToString());
            }

        }

        private Int32 ObterIdPorTipoECPF(int tipo, string cpf)
        {

            try
            {


                Oracle.ManagedDataAccess.Client.OracleParameter idGerado = sd.CriaOUTParam(9, "p_id");

                sd.ExecutaProc("pkg_colaborador.sp_colaborador_id_por_tipo_cpf", tipo, cpf, idGerado);

                return Int32.Parse(idGerado.Value.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("[ObterIdPorTipoECPF] " + ex.ToString());
            }

        }

        private Colaborador ObterDadosDoColaboradorTipo(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("pkg_colaborador.sp_colaborador_tipo", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {
                    colaborador.Id = int.Parse(dtResultados.Rows[0]["id_colaborador"].ToString());
                    colaborador.Ativo = dtResultados.Rows[0]["IND_ATIVO"].ToString() == "S";
                    colaborador.TipoColaborador = new TipoColaborador { Id = int.Parse(dtResultados.Rows[0]["id_tipo_colaborador"].ToString()), Descricao = dtResultados.Rows[0]["desc_tipo_colaborador"].ToString() };

                    colaborador.Deficiencia.EhPortadorDeNecessidadeEspecial = dtResultados.Rows[0]["ind_port_necessidade"].ToString().ToUpper() == "S";
                    colaborador.Deficiencia.EhReabilitadoReadaptado = dtResultados.Rows[0]["ind_reab_readaptado"].ToString().ToUpper() == "S";
                    colaborador.Deficiencia.ObservacaoDeficiencia = dtResultados.Rows[0]["obs_deficiencia"].ToString();


                    return colaborador;
                }
                return null;

            }
            catch (Exception ex)
            {
                throw new Exception("[ObterDadosDoColaboradorTipo] " + ex.ToString());
            }

        }


        private Colaborador ObterRemuneracao(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("pkg_colaborador.sp_colaborador_remuneracao", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["salario_base"].ToString()))
                        colaborador.SalarioBase = double.Parse(dtResultados.Rows[0]["salario_base"].ToString());
                    colaborador.ObservacaoRemuneracao = dtResultados.Rows[0]["observacao_remuneracao"].ToString();

                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["id_unidade_pag"].ToString()))
                        colaborador.UnidadePagamento = new UnidadePagamento { Id = int.Parse(dtResultados.Rows[0]["id_unidade_pag"].ToString()), Descricao = dtResultados.Rows[0]["desc_unidade_pag"].ToString() };

                }
                return colaborador;

            }
            catch (Exception ex)
            {
                throw new Exception("[ObterRemuneracao] " + ex.ToString());
            }

        }


        private Colaborador ObterImigrante(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("pkg_colaborador.sp_colaborador_imigrante", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {
                    colaborador.ClassificacaoTrabEstrangeiro = new ClassificacaoTrabEstrangeiro { Id = int.Parse(dtResultados.Rows[0]["id_classificacao"].ToString()), Descricao = dtResultados.Rows[0]["descricao_classificacao"].ToString() };
                    colaborador.TempoResidenciaEstrangeiro = new TempoResidenciaEstrangeiro { Id = int.Parse(dtResultados.Rows[0]["id_tempo_residente"].ToString()), Descricao = dtResultados.Rows[0]["descricao_tempo_residente"].ToString() };

                }
                return colaborador;

            }
            catch (Exception ex)
            {
                throw new Exception("[ObterImigrante] " + ex.ToString());
            }

        }

        private Colaborador ObterResidenteExterior(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("pkg_colaborador.sp_colaborador_res_ext", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["id_pais"].ToString()))
                        colaborador.ResidenciaExterior.PaisResidencia = new Pais { Id = int.Parse(dtResultados.Rows[0]["id_pais"].ToString()), Descricao = dtResultados.Rows[0]["nome_pais"].ToString() };
                    colaborador.ResidenciaExterior.Descricao = dtResultados.Rows[0]["logradouro"].ToString();
                    colaborador.ResidenciaExterior.Numero = dtResultados.Rows[0]["numero"].ToString();
                    colaborador.ResidenciaExterior.Complemento = dtResultados.Rows[0]["complemento"].ToString();
                    colaborador.ResidenciaExterior.Bairro = dtResultados.Rows[0]["bairro"].ToString();
                    colaborador.ResidenciaExterior.Cidade = dtResultados.Rows[0]["cidade"].ToString();
                    colaborador.ResidenciaExterior.CodigoPostal = dtResultados.Rows[0]["codigo_postal"].ToString();
                }
                return colaborador;

            }
            catch (Exception ex)
            {
                throw new Exception("[ObterImigrante] " + ex.ToString());
            }

        }


        public Colaborador ObterDependente(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_dependente", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {

                    colaborador.Dependentes = (from DataRow dr in dtResultados.Rows
                                               select new Dependente
                                               {
                                                   idDependenteAux = int.Parse(dr["id"].ToString()),
                                                   Nome = dr["nome"].ToString(),
                                                   Nascimento = DateTime.Parse(dr["data_nascimento"].ToString()).ToString("dd/MM/yyyy"),
                                                   CPF = dr["cpf"].ToString(),
                                                   TemIRRF = dr["ind_irrf"].ToString() == "S",
                                                   TemSalarioFamilia = dr["ind_salario_familia"].ToString() == "S",
                                                   TemIncapacidadeFisicaMental = dr["ind_incapacidade"].ToString() == "S",
                                                   TipoDependente = { Id = int.Parse(dr["id_tipo_dependente"].ToString()), Descricao = dr["descricao_tipo_dependente"].ToString() },
                                               }).ToList();
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[ObterDependente] " + ex.ToString());
            }

        }


        private Colaborador ObterDadosTSVE(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("pkg_colaborador.sp_colaborador_tsve", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {
                    colaborador.DadoTSVE.CadastramentoInicial = new CadastramentoInicial { Id = int.Parse(dtResultados.Rows[0]["id_cadastramento_inicial"].ToString()), Descricao = dtResultados.Rows[0]["desc_cadastramento_inicial"].ToString() };
                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["id_categoria_trabalhador"].ToString()))
                        colaborador.DadoTSVE.CategoriaTrabalhador = new CategoriaTrabalhador { Id = int.Parse(dtResultados.Rows[0]["id_categoria_trabalhador"].ToString()), Descricao = dtResultados.Rows[0]["desc_categoria_trabalhador"].ToString() };
                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["id_natureza_atividade"].ToString()))
                        colaborador.DadoTSVE.NaturezaAtividade = new NaturezaAtividade { Id = int.Parse(dtResultados.Rows[0]["id_natureza_atividade"].ToString()), Descricao = dtResultados.Rows[0]["desc_natureza_atividade"].ToString() };

                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["data_inicio"].ToString()))
                        colaborador.DadoTSVE.DataInicio = DateTime.Parse(dtResultados.Rows[0]["data_inicio"].ToString()).ToString("dd/MM/yyyy");

                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["numero_processo_trabalho"].ToString()))
                        colaborador.DadoTSVE.NumeroProcesso = dtResultados.Rows[0]["numero_processo_trabalho"].ToString();

                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["matricula"].ToString()))
                        colaborador.DadoTSVE.Matricula = dtResultados.Rows[0]["matricula"].ToString();

                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["cargo"].ToString()))
                        colaborador.DadoTSVE.Cargo = dtResultados.Rows[0]["cargo"].ToString();

                    if (!string.IsNullOrEmpty(dtResultados.Rows[0]["id_cbo"].ToString()))
                        colaborador.DadoTSVE.CBOCargo = new CBOCargo { Id = int.Parse(dtResultados.Rows[0]["id_cbo"].ToString()), Descricao = dtResultados.Rows[0]["descricao_cbo"].ToString() };

                }
                return colaborador;

            }
            catch (Exception ex)
            {
                throw new Exception("[ObterResidenteExterior] " + ex.ToString());
            }

        }


        private Colaborador ObterInformacaoEstagio(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("pkg_colaborador.sp_colaborador_inf_est", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {
                    colaborador.DadoEstagio.NaturezaEstagio = new NaturezaEstagio { Id = int.Parse(dtResultados.Rows[0]["natureza"].ToString()), Descricao = dtResultados.Rows[0]["descricao_natureza"].ToString() };
                    colaborador.DadoEstagio.NivelEstagio = new NivelEstagio { Id = int.Parse(dtResultados.Rows[0]["nivel"].ToString()), Descricao = dtResultados.Rows[0]["descricao_nivel"].ToString() };

                    colaborador.DadoEstagio.SetorEstagio = new Setor { Id = int.Parse(dtResultados.Rows[0]["id_setor"].ToString()), Descricao = dtResultados.Rows[0]["descricao_setor"].ToString() };

                    colaborador.DadoEstagio.AreaAtuacao = dtResultados.Rows[0]["area_atuacao"].ToString();
                    colaborador.DadoEstagio.ApoliceSeguro = dtResultados.Rows[0]["numero_apolice"].ToString();
                    colaborador.DadoEstagio.DataPrevistaTermino = DateTime.Parse(dtResultados.Rows[0]["data_prevista_termino"].ToString()).ToString("dd/MM/yyyy");
                    colaborador.DadoEstagio.DataInicio = DateTime.Parse(dtResultados.Rows[0]["data_inicio"].ToString()).ToString("dd/MM/yyyy");
                    colaborador.DadoEstagio.CPFSupervisor = dtResultados.Rows[0]["cpf_superior"].ToString();
                    colaborador.DadoEstagio.NomeSupervisor = dtResultados.Rows[0]["nome_superior"].ToString();

                    colaborador.DadoEstagio.HorarioInicio = dtResultados.Rows[0]["horario_inicio"].ToString();
                    colaborador.DadoEstagio.HorarioTermino = dtResultados.Rows[0]["horario_final"].ToString();
                    colaborador.DadoEstagio.EHorarioDinamico = dtResultados.Rows[0]["ind_hora_dinamica"].ToString() == "S";


                }
                return colaborador;

            }
            catch (Exception ex)
            {
                throw new Exception("[ObterInformacaoEstagio] " + ex.ToString());
            }

        }

        public Colaborador ObterAfastamento(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_afastamento", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {

                    colaborador.Afastamentos = (from DataRow dr in dtResultados.Rows
                                                select new Afastamento
                                                {
                                                    DataInicio = string.IsNullOrEmpty(dr["data_inicio"].ToString()) ? "" : DateTime.Parse(dr["data_inicio"].ToString()).ToString("dd/MM/yyyy"),
                                                    DataTermino = string.IsNullOrEmpty(dr["data_termino"].ToString()) ? "" : DateTime.Parse(dr["data_termino"].ToString()).ToString("dd/MM/yyyy"),
                                                    ObservacaoAfastamento = dr["observacao"].ToString(),
                                                    MotivoAfastamento = { Id = int.Parse(dr["id_motivo_afastamento"].ToString()), Descricao = dr["descr_motivo_afastamento"].ToString() },
                                                }).ToList();
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[ObterAfastamento] " + ex.ToString());
            }

        }

        public Colaborador ObterDadosProfissionais(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_exp_prof", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {

                    colaborador.ExperienciasProfissional = (from DataRow dr in dtResultados.Rows
                                                            select new DadoProfissional
                                                            {
                                                                Ano = int.Parse(dr["ano"].ToString()),
                                                                Empresa = dr["empresa"].ToString(),
                                                                CargoOcupado = dr["cargo"].ToString(),
                                                                DataInicioPeriodo = string.IsNullOrEmpty(dr["data_inicio_periodo"].ToString()) ? "" : DateTime.Parse(dr["data_inicio_periodo"].ToString()).ToString("dd/MM/yyyy"),
                                                                DataFimPeriodo = string.IsNullOrEmpty(dr["data_fim_periodo"].ToString()) ? "" : DateTime.Parse(dr["data_fim_periodo"].ToString()).ToString("dd/MM/yyyy"),
                                                            }).ToList();
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[ObterDadosProfissionais] " + ex.ToString());
            }

        }

        public Colaborador ObterDadosCursoExtra(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_curs_extra", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {

                    colaborador.CursosExtraCurriculares = (from DataRow dr in dtResultados.Rows
                                                           select new CursoExtraCurricular
                                                           {
                                                               Descricao = dr["descricao"].ToString(),
                                                           }).ToList();
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[ObterDadosCursoExtra] " + ex.ToString());
            }

        }

        private Colaborador ObterInstituicaoEnsino(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("pkg_colaborador.sp_colaborador_inst_ens", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {
                    colaborador.DadoEstagio.EnderecoInstEnsino.UF = new UnidadeFederacao { Id = dtResultados.Rows[0]["uf"].ToString(), Descricao = dtResultados.Rows[0]["uf_desc"].ToString() };
                    colaborador.DadoEstagio.EnderecoInstEnsino.TipoLogradouro = new TipoLogradouro { Id = int.Parse(dtResultados.Rows[0]["id_tipo_logradouro"].ToString()), Descricao = dtResultados.Rows[0]["desc_tipo_logradouro"].ToString() };

                    colaborador.DadoEstagio.CNPJInstEnsino = dtResultados.Rows[0]["cnpj"].ToString();
                    colaborador.DadoEstagio.RazaoSocialInstEnsino = dtResultados.Rows[0]["razao_social"].ToString();
                    colaborador.DadoEstagio.EnderecoInstEnsino.NomeLogradouro = dtResultados.Rows[0]["logradouro"].ToString();

                    colaborador.DadoEstagio.EnderecoInstEnsino.NumeroLogradouro = dtResultados.Rows[0]["numero"].ToString();
                    colaborador.DadoEstagio.EnderecoInstEnsino.Complemento = dtResultados.Rows[0]["complemento"].ToString();
                    colaborador.DadoEstagio.EnderecoInstEnsino.Bairro = dtResultados.Rows[0]["bairro"].ToString();
                    colaborador.DadoEstagio.EnderecoInstEnsino.Municipio = dtResultados.Rows[0]["municipio"].ToString();
                    colaborador.DadoEstagio.EnderecoInstEnsino.CEP = dtResultados.Rows[0]["cep"].ToString();

                    colaborador.DadoEstagio.CNPJAgenteInt = dtResultados.Rows[0]["cnpj_agente"].ToString();
                    colaborador.DadoEstagio.RazaoSocialAgenteInt = dtResultados.Rows[0]["nome_agente"].ToString();



                }
                return colaborador;

            }
            catch (Exception ex)
            {
                throw new Exception("[ObterInstituicaoEnsino] " + ex.ToString());
            }

        }

        public Colaborador ObterDadoGeral(Colaborador colaborador)
        {

            try
            {
                if (colaborador.Id > 0 || !string.IsNullOrEmpty(colaborador.CPF))
                {
                    var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_dado_basico", colaborador.Id, colaborador.CPF, sd.CriaRefCursor()).Tables[0];

                    if (dtResultados.Rows.Count > 0)
                    {
                        //DADOS BÁSICOS
                        colaborador.Id = int.Parse(dtResultados.Rows[0]["ID"].ToString());
                        colaborador.CPF = dtResultados.Rows[0]["CPF"].ToString();
                        colaborador.Nome = dtResultados.Rows[0]["NOME"].ToString();
                        colaborador.RG = dtResultados.Rows[0]["RG"].ToString();

                        colaborador.Sexo = dtResultados.Rows[0]["SEXO"].ToString();
                        colaborador.Raca = new Raca { Id = int.Parse(dtResultados.Rows[0]["ID_RACA"].ToString()), Descricao = dtResultados.Rows[0]["DESCRICAO_RACA"].ToString() };
                        colaborador.EstadoCivil = new EstadoCivil { Id = int.Parse(dtResultados.Rows[0]["ID_ESTADO_CIVIL"].ToString()), Descricao = dtResultados.Rows[0]["DESCRICAO_ESTADO_CIVIL"].ToString() };
                        colaborador.GrauInstrucao = new GrauInstrucao { Id = int.Parse(dtResultados.Rows[0]["ID_INSTRUCAO"].ToString()), Descricao = dtResultados.Rows[0]["DESCRICAO_INSTRUCAO"].ToString() };
                        colaborador.NomeSocial = dtResultados.Rows[0]["NOME_SOCIAL"].ToString();
                        colaborador.Nascimento = DateTime.Parse(dtResultados.Rows[0]["DATA_NASCIMENTO"].ToString()).ToString("dd/MM/yyyy");
                        colaborador.PaisNascimento = new Pais { Id = int.Parse(dtResultados.Rows[0]["ID_PAIS_NASCIMENTO"].ToString()), Descricao = dtResultados.Rows[0]["DESCRICAO_PAIS_NASCIMENTO"].ToString() };
                        colaborador.PaisNacionalidade = new Pais { Id = int.Parse(dtResultados.Rows[0]["ID_PAIS_NACIONALIDADE"].ToString()), Descricao = dtResultados.Rows[0]["DESCRICAO_PAIS_NACIONALIDADE"].ToString() };
                        colaborador.IDGed = dtResultados.Rows[0]["id_ged"].ToString();

                        if (!string.IsNullOrEmpty(dtResultados.Rows[0]["data_alt_cpf"].ToString()) && !string.IsNullOrEmpty(dtResultados.Rows[0]["cpf_antigo"].ToString()))
                        {
                            colaborador.CPFAntigo = dtResultados.Rows[0]["cpf_antigo"].ToString();
                            colaborador.DataAlteracaoCPF = DateTime.Parse(dtResultados.Rows[0]["data_alt_cpf"].ToString()).ToString("dd/MM/yyyy");
                            colaborador.ObservacaoAlteracaoCPF = dtResultados.Rows[0]["obs_alt_cpf"].ToString();
                        }

                        colaborador.HorarioInicioTrabalho = dtResultados.Rows[0]["horario_inicio_trabalho"].ToString();
                        colaborador.HorarioTerminoTrabalho = dtResultados.Rows[0]["horario_final_trabalho"].ToString();
                        colaborador.EHorarioDinamicoTrabalho = dtResultados.Rows[0]["ind_hora_dinamica_trabalho"].ToString() == "S";

                        return colaborador;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("[DadoBasico] " + ex.ToString());
            }

        }

        public Colaborador ObterDeficiencia(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_deficiencia", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {

                    colaborador.Deficiencia.ClassificacaoDeficiencias = (from DataRow dr in dtResultados.Rows
                                                                         select new ClassificacaoDeficiencia
                                                                         {
                                                                             Id = int.Parse(dr["ID"].ToString()),
                                                                             Descricao = dr["DESCRICAO"].ToString()
                                                                         }).ToList();

                }
                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[Deficiencia] " + ex.ToString());
            }

        }

        public Colaborador ObterEndereco(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_endereco", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {

                    colaborador.Enderecos = (from DataRow dr in dtResultados.Rows
                                             select new Endereco
                                             {
                                                 CEP = dr["CEP"].ToString(),
                                                 TipoLogradouro = { Id = int.Parse(dr["id_tipo_logradouro"].ToString()), Descricao = dr["descricao_tipo_logradouro"].ToString() },
                                                 NomeLogradouro = dr["LOGRADOURO"].ToString(),
                                                 NumeroLogradouro = dr["NUMERO"].ToString(),
                                                 Complemento = dr["COMPLEMENTO"].ToString(),
                                                 Bairro = dr["BAIRRO"].ToString(),
                                                 Municipio = dr["MUNICIPIO"].ToString(),
                                                 UF = { Id = dr["UF"].ToString(), Descricao = dr["descricao_uf"].ToString() },
                                                 Tipo = (TipoEnderecoEnum)int.Parse(dr["tipo_endereco"].ToString())
                                             }).ToList();
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[Endereco] " + ex.ToString());
            }

        }

        public Colaborador ObterEmail(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_email", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {

                    colaborador.Emails = (from DataRow dr in dtResultados.Rows
                                          select new EmailPessoa
                                          {
                                              Email = dr["EMAIL"].ToString(),
                                              TipoContato = (TipoContatoEnum)int.Parse(dr["tipo_contato"].ToString())
                                          }).ToList();
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[Email] " + ex.ToString());
            }

        }

        public Colaborador ObterTelefone(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_telefone", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {

                    colaborador.Telefones = (from DataRow dr in dtResultados.Rows
                                             select new TelefonePessoa
                                             {
                                                 DDD = int.Parse(dr["ddd"].ToString()),
                                                 Numero = int.Parse(dr["numero"].ToString()),
                                                 TipoContato = (TipoContatoEnum)int.Parse(dr["tipo_contato"].ToString())
                                             }).ToList();
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[Telefone] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }

        public Colaborador ObterInformacoesAcademica(Colaborador colaborador)
        {

            try
            {
                var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_inf_academica", colaborador.IdDoTipo, sd.CriaRefCursor()).Tables[0];

                if (dtResultados.Rows.Count > 0)
                {

                    colaborador.InformacoesAcademica = (from DataRow dr in dtResultados.Rows
                                                       select new InformacoesAcademica
                                                       {
                                                            Curso = dr["curso"].ToString(),
                                                            Situacao = { Id = int.Parse(dr["id_situacao"].ToString()), Descricao = dr["descricao_situacao"].ToString() },
                                                            PrevisaoConclusao = dr["previsao_conclusao"].ToString(),
                                                       }).ToList();
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[ObterInformacoesAcademica] " + ex.ToString());
            }

        }


        #endregion

        #region Inclusão
        private Int32 IncluirDadoGeral(Colaborador colaborador)
        {
            Oracle.ManagedDataAccess.Client.OracleParameter idGerado = sd.CriaOUTParam(9, "p_id");

            sd.ExecutaProc("pkg_colaborador.sp_colaborador_dado_basico_inc",
                colaborador.CPF,
                colaborador.Nome,
                colaborador.RG,
                colaborador.Sexo,
                colaborador.Raca.Id,
                colaborador.EstadoCivil.Id,
                colaborador.GrauInstrucao.Id,
                colaborador.NomeSocial,
                colaborador.NascimentoConversao(),
                colaborador.PaisNascimento.Id,
                colaborador.PaisNacionalidade.Id,
                colaborador.IDGed,
                colaborador.CPFAntigo,
                colaborador.DataAlteracaoCPFConversao() == DateTime.MinValue ? null : colaborador.DataAlteracaoCPFConversao(),
                colaborador.ObservacaoAlteracaoCPF,
                colaborador.HorarioInicioTrabalho,
                colaborador.HorarioTerminoTrabalho,
                TrataBoolean(colaborador.EHorarioDinamicoTrabalho),
                idGerado);


            return Int32.Parse(idGerado.Value.ToString());
        }

        private Int32 IncluirTipo(Colaborador colaborador)
        {
            Oracle.ManagedDataAccess.Client.OracleParameter idGerado = sd.CriaOUTParam(9, "p_id");

            sd.ExecutaProc("pkg_colaborador.sp_colaborador_tipo_inc",
                            colaborador.Id, 
                            colaborador.TipoColaborador.Id, 
                            TrataBoolean(colaborador.Ativo), 
                            TrataBoolean(colaborador.Deficiencia.EhPortadorDeNecessidadeEspecial),
                            TrataBoolean(colaborador.Deficiencia.EhReabilitadoReadaptado), 
                            colaborador.Deficiencia.ObservacaoDeficiencia, 
                            idGerado);

            return Int32.Parse(idGerado.Value.ToString());

        }

        private void IncluirRemuneracao(Colaborador colaborador)
        {
            if (colaborador.SalarioBase != null && colaborador.UnidadePagamento.Id > 0)
                sd.ExecutaProc("pkg_colaborador.sp_colaborador_remuneracao_inc",
                    colaborador.IdDoTipo, colaborador.SalarioBase, colaborador.UnidadePagamento.Id, colaborador.ObservacaoRemuneracao);
        }

        private void IncluirImigrante(Colaborador colaborador)
        {
            if (colaborador.TempoResidenciaEstrangeiro.Id > 0 && colaborador.ClassificacaoTrabEstrangeiro.Id > 0)
                sd.ExecutaProc("pkg_colaborador.sp_colaborador_imigrante_inc",
                colaborador.IdDoTipo, colaborador.TempoResidenciaEstrangeiro.Id, colaborador.ClassificacaoTrabEstrangeiro.Id);
        }

        private void IncluirResidenteExterior(Colaborador colaborador)
        {
            // só grava se não informar endereço
            //if (!colaborador.Enderecos.Any())
            //{
            sd.ExecutaProc("pkg_colaborador.sp_colaborador_res_exter_inc",
                                     TratarZeroComoNulo(colaborador.ResidenciaExterior.PaisResidencia.Id),
                                     colaborador.ResidenciaExterior.Descricao,
                                     colaborador.ResidenciaExterior.Numero,
                                     colaborador.ResidenciaExterior.Complemento,
                                     colaborador.ResidenciaExterior.Bairro,
                                     colaborador.ResidenciaExterior.Cidade,
                                     colaborador.ResidenciaExterior.CodigoPostal,
                                     colaborador.IdDoTipo);
            //}

        }

        private void IncluirInformacaoEstagio(Colaborador colaborador)
        {
            //só grava para estagiário
            if (colaborador.TipoColaborador.Id == 1)
            {
                sd.ExecutaProc("pkg_colaborador.sp_colaborador_inf_estagio_inc",
                                                         colaborador.DadoEstagio.NaturezaEstagio.Id,
                                                         colaborador.DadoEstagio.NivelEstagio.Id,
                                                         colaborador.DadoEstagio.AreaAtuacao,
                                                         colaborador.DadoEstagio.ApoliceSeguro,
                                                         colaborador.DadoEstagio.DataPrevistaTerminoConversao(),
                                                         colaborador.DadoEstagio.CPFSupervisor,
                                                         colaborador.DadoEstagio.NomeSupervisor,
                                                         colaborador.DadoEstagio.SetorEstagio.Id,
                                                         colaborador.DadoEstagio.HorarioInicio,
                                                         colaborador.DadoEstagio.HorarioTermino,
                                                         colaborador.DadoEstagio.DataInicioConversao(),
                                                         colaborador.IdDoTipo,
                                                          TrataBoolean(colaborador.DadoEstagio.EHorarioDinamico));
            }
        }

        private void IncluirInstituicaoDeEnsino(Colaborador colaborador)
        {
            //só grava para estagiário
            if (colaborador.TipoColaborador.Id == 1)
            {
                sd.ExecutaProc("pkg_colaborador.sp_colaborador_inst_estag_inc",
                                                         colaborador.DadoEstagio.CNPJInstEnsino,
                                                         colaborador.DadoEstagio.RazaoSocialInstEnsino,
                                                         colaborador.DadoEstagio.EnderecoInstEnsino.TipoLogradouro.Id,
                                                         colaborador.DadoEstagio.EnderecoInstEnsino.NomeLogradouro,
                                                         colaborador.DadoEstagio.EnderecoInstEnsino.NumeroLogradouro,
                                                         colaborador.DadoEstagio.EnderecoInstEnsino.Complemento,
                                                         colaborador.DadoEstagio.EnderecoInstEnsino.Bairro,
                                                         colaborador.DadoEstagio.EnderecoInstEnsino.Municipio,
                                                         colaborador.DadoEstagio.EnderecoInstEnsino.UF.Id,
                                                         colaborador.DadoEstagio.EnderecoInstEnsino.CEP,
                                                         colaborador.DadoEstagio.CNPJAgenteInt,
                                                         colaborador.DadoEstagio.RazaoSocialAgenteInt,
                                                         colaborador.IdDoTipo);
            }
        }

        private void IncluirDadosTSVE(Colaborador colaborador)
        {

            sd.ExecutaProc("pkg_colaborador.sp_colaborador_tsve_inc",
                                                     colaborador.DadoTSVE.CadastramentoInicial.Id,
                                                     TratarZeroComoNulo(colaborador.DadoTSVE.CategoriaTrabalhador.Id),
                                                     TratarZeroComoNulo(colaborador.DadoTSVE.NaturezaAtividade.Id),
                                                     colaborador.DadoTSVE.Matricula,
                                                     colaborador.DadoTSVE.NumeroProcesso,
                                                     colaborador.DadoTSVE.DataInicioConversao(),
                                                     colaborador.DadoTSVE.Cargo,
                                                     TratarZeroComoNulo(colaborador.DadoTSVE.CBOCargo.Id),
                                                     colaborador.IdDoTipo);

        }



        private void IncluirDeficiencia(Colaborador colaborador)
        {

            foreach (ClassificacaoDeficiencia deficiencia in colaborador.Deficiencia.ClassificacaoDeficiencias)
            {
                sd.ExecutaProc("pkg_colaborador.sp_colaborador_deficiencia_inc",
                    colaborador.IdDoTipo, deficiencia.Id);
            }
        }

        private void IncluirEmail(Colaborador colaborador)
        {
            foreach (EmailPessoa email in colaborador.Emails)
            {
                sd.ExecutaProc("pkg_colaborador.sp_colaborador_email_inc",
                    colaborador.IdDoTipo, email.Email, (int)email.TipoContato);
            }
        }

        private void IncluirTelefone(Colaborador colaborador)
        {
            foreach (TelefonePessoa telefone in colaborador.Telefones)
            {
                if (telefone.DDD != null && telefone.Numero != null)
                    sd.ExecutaProc("pkg_colaborador.sp_colaborador_telefone_inc",
                    colaborador.IdDoTipo, telefone.DDD, telefone.Numero, (int)telefone.TipoContato);
            }
        }

        private void IncluirEndereco(Colaborador colaborador)
        {
            foreach (Endereco endereco in colaborador.Enderecos)
            {
                sd.ExecutaProc("pkg_colaborador.sp_colaborador_endereco_inc",
                   endereco.TipoLogradouro.Id, endereco.NomeLogradouro, endereco.NumeroLogradouro, endereco.Complemento, endereco.Bairro, endereco.Municipio, endereco.UF.Id, endereco.CEP, (int)endereco.Tipo, colaborador.IdDoTipo);
            }
        }

        private void IncluirDependente(Colaborador colaborador)
        {
            foreach (Dependente dependente in colaborador.Dependentes)
            {

                sd.ExecutaProc("pkg_colaborador.sp_colaborador_dependente_inc",
                    dependente.Nome, dependente.NascimentoConversao(), dependente.CPF, dependente.TipoDependente.Id,
                    TrataBoolean(dependente.TemIRRF), TrataBoolean(dependente.TemSalarioFamilia), TrataBoolean(dependente.TemIncapacidadeFisicaMental),
                   colaborador.IdDoTipo);

            }
        }

        private void IncluirAfastamento(Colaborador colaborador)
        {
            foreach (Afastamento afastamento in colaborador.Afastamentos)
            {
                sd.ExecutaProc("pkg_colaborador.sp_colaborador_afastamento_inc",
                    afastamento.DataInicioConversao() == DateTime.MinValue ? null : afastamento.DataInicioConversao(),
                    afastamento.DataTerminoConversao() == DateTime.MinValue ? null : afastamento.DataTerminoConversao(),
                    afastamento.ObservacaoAfastamento,
                    afastamento.MotivoAfastamento.Id,
                    colaborador.IdDoTipo);

            }
        }

        private void IncluirDadosProfissionais(Colaborador colaborador)
        {
            foreach (DadoProfissional dadoProfissional in colaborador.ExperienciasProfissional)
            {

                sd.ExecutaProc("pkg_colaborador.sp_colaborador_exp_prof_inc",
                   dadoProfissional.Ano, 
                   dadoProfissional.Empresa, 
                   dadoProfissional.CargoOcupado, 
                   dadoProfissional.DataInicioPeriodoConversao() == DateTime.MinValue ? null : dadoProfissional.DataInicioPeriodoConversao(),
                   dadoProfissional.DataFimPeriodoConversao() == DateTime.MinValue ? null : dadoProfissional.DataFimPeriodoConversao(),
                   colaborador.IdDoTipo);

            }
        }

        private void IncluirCursoExtraCurricular(Colaborador colaborador)
        {
            foreach (CursoExtraCurricular cursoExtraCurricular in colaborador.CursosExtraCurriculares)
            {

                sd.ExecutaProc("pkg_colaborador.sp_colaborador_curso_exta_inc",
                  cursoExtraCurricular.Descricao, colaborador.IdDoTipo);

            }
        }


        private void IncluirInformacoesAcademica(Colaborador colaborador)
        {
            foreach (InformacoesAcademica informacoesAcademica in colaborador.InformacoesAcademica)
            {
                sd.ExecutaProc("pkg_colaborador.sp_colaborador_inf_acad_inc", 
                               informacoesAcademica.Curso, informacoesAcademica.Situacao.Id, 
                               informacoesAcademica.PrevisaoConclusao, colaborador.IdDoTipo);
            }
        }
        #endregion

        #region Alteração
        private void AlterarDadoGeral(Colaborador colaborador)
        {
            sd.ExecutaProc("pkg_colaborador.sp_colaborador_dado_basico_alt",
                colaborador.Id,
                colaborador.CPF,
                colaborador.Nome,
                colaborador.RG,
                colaborador.Sexo,
                colaborador.Raca.Id,
                colaborador.EstadoCivil.Id,
                colaborador.GrauInstrucao.Id,
                colaborador.NomeSocial,
                colaborador.NascimentoConversao(),
                colaborador.PaisNascimento.Id,
                colaborador.PaisNacionalidade.Id,
                colaborador.IDGed,
                colaborador.CPFAntigo ?? "",
                colaborador.DataAlteracaoCPFConversao() == DateTime.MinValue ? null : colaborador.DataAlteracaoCPFConversao(),
                colaborador.ObservacaoAlteracaoCPF ?? "",
                colaborador.HorarioInicioTrabalho,
                colaborador.HorarioTerminoTrabalho,
                TrataBoolean(colaborador.EHorarioDinamicoTrabalho)
                );
        }

        private Int32 AlterarTipo(Colaborador colaborador)
        {
            Oracle.ManagedDataAccess.Client.OracleParameter idColaboradorTipo = sd.CriaOUTParam(9, "p_id");

            sd.ExecutaProc("pkg_colaborador.sp_colaborador_tipo_alt",
                colaborador.Id, colaborador.TipoColaborador.Id, TrataBoolean(colaborador.Ativo), TrataBoolean(colaborador.Deficiencia.EhPortadorDeNecessidadeEspecial),
                TrataBoolean(colaborador.Deficiencia.EhReabilitadoReadaptado), colaborador.Deficiencia.ObservacaoDeficiencia, idColaboradorTipo);

            return Int32.Parse(idColaboradorTipo.Value.ToString());
        }

        public void GravarIDDoGed(Colaborador colaborador)
        {
            try
            {
                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }

                trans = sd.BeginTransaction();

                sd.ExecutaProc("pkg_colaborador.sp_colaborador_grava_ged", colaborador.Id, colaborador.IDGed);

                trans.Commit();
            }
            catch (ServicoDadosODPNETM.ServicoDadosException sdEx)
            {
                trans.Rollback();
                throw new Exception(sdEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                sd.Connection.Close();
            }

        }

        #endregion


        public Colaborador PreencherTrocaDeCPFValores(Colaborador colaborador)
        {

            try
            {
                if (colaborador.Id > 0)
                {
                    var dtResultados = sd.ExecutaProcDS("PKG_COLABORADOR.sp_colaborador_dado_basico", colaborador.Id, colaborador.CPF, sd.CriaRefCursor()).Tables[0];

                    if (dtResultados.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtResultados.Rows[0]["data_alt_cpf"].ToString()) && !string.IsNullOrEmpty(dtResultados.Rows[0]["cpf_antigo"].ToString()))
                        {
                            colaborador.CPFAntigo = dtResultados.Rows[0]["cpf_antigo"].ToString();
                            colaborador.DataAlteracaoCPF = DateTime.Parse(dtResultados.Rows[0]["data_alt_cpf"].ToString()).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            colaborador.CPFAntigo = null;
                            colaborador.DataAlteracaoCPF = "";
                        }
                    }
                    else
                    {
                        colaborador.CPFAntigo = null;
                        colaborador.DataAlteracaoCPF = "";
                    }
                }
                else
                {
                    colaborador.CPFAntigo = null;
                    colaborador.DataAlteracaoCPF = "";
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception("[PreencherTrocaDeCPFValores] " + ex.ToString());
            }
        }

    }
}
