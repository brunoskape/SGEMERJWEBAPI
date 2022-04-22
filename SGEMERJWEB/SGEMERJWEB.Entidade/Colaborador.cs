using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using System;
using System.Collections.Generic;

namespace SGEMERJWEB.Entidade
{
    public class Colaborador : EntidadeBase
    {
        public Colaborador()
        {
            Raca = new Raca();
            EstadoCivil = new EstadoCivil();
            GrauInstrucao = new GrauInstrucao();
            TipoColaborador = new TipoColaborador();
            PaisNacionalidade = new Pais();
            PaisNascimento = new Pais();
            Deficiencia = new Deficiencia();
            Enderecos = new List<Endereco>();
            Emails = new List<EmailPessoa>();
            Telefones = new List<TelefonePessoa>();
            UnidadePagamento = new UnidadePagamento();
            TempoResidenciaEstrangeiro = new TempoResidenciaEstrangeiro();
            ClassificacaoTrabEstrangeiro = new ClassificacaoTrabEstrangeiro();
            Dependentes = new List<Dependente>();

            ExperienciasProfissional = new List<DadoProfissional>();
            CursosExtraCurriculares = new List<CursoExtraCurricular>();

            DadoEstagio = new DadoEstagio();

            ResidenciaExterior = new ResidenciaExterior();

            Afastamentos = new List<Afastamento>();

            DadoTSVE = new DadoTSVE();

            Rescisao = new Rescisao();

            InformacoesAcademica = new List<InformacoesAcademica>();
        }

        #region Dados Básicos
        public int IdDoTipo { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string NomeSocial { get; set; }
        public bool Ativo { get; set; }
        public Raca Raca { get; set; }
        public EstadoCivil EstadoCivil { get; set; }
        public GrauInstrucao GrauInstrucao { get; set; }
        public TipoColaborador TipoColaborador { get; set; }
        public string Sexo { get; set; }
        public string HorarioInicioTrabalho { get; set; }
        public string HorarioTerminoTrabalho { get; set; }
        public bool EHorarioDinamicoTrabalho { get; set; }
        #endregion

        #region Mudança de CPF
        public string CPFNovo { get; set; }
        public string CPFAntigo { get; set; }
        public string DataAlteracaoCPF { get; set; }
        public string ObservacaoAlteracaoCPF { get; set; }
        #endregion

        #region Nascimento
        public string Nascimento { get; set; }
        public Pais PaisNacionalidade { get; set; }
        public Pais PaisNascimento { get; set; }
        #endregion

        #region Deficiência Pessoa
        public Deficiencia Deficiencia { get; set; }
        #endregion

        #region Endereço
        public List<Endereco> Enderecos { get; set; }
        #endregion

        #region Contatos
        public List<EmailPessoa> Emails { get; set; }
        public List<TelefonePessoa> Telefones { get; set; }
        #endregion
       
        #region Remuneração
        public double? SalarioBase { get; set; }
        public UnidadePagamento UnidadePagamento { get; set; }
        public string ObservacaoRemuneracao { get; set; }
        #endregion

        #region Trabalhador Estrangeiro
        public TempoResidenciaEstrangeiro TempoResidenciaEstrangeiro { get; set; }
        public ClassificacaoTrabEstrangeiro ClassificacaoTrabEstrangeiro { get; set; }
        #endregion

        #region Residente no Exterior
       public ResidenciaExterior ResidenciaExterior { get; set; }
        #endregion

        #region Dependentes
        public List<Dependente> Dependentes { get; set; }
        #endregion

        #region Dados Profissionais
        public List<DadoProfissional> ExperienciasProfissional { get; set; }
        public List<CursoExtraCurricular> CursosExtraCurriculares { get; set; }
        #endregion

        #region Afastamento
        public List<Afastamento> Afastamentos { get; set; }
        #endregion

        #region Informação Estágio
        public DadoEstagio DadoEstagio { get; set; }
        #endregion

        #region Dados TSVE
        public DadoTSVE DadoTSVE { get; set; }
        #endregion     



        #region Foto no GED
        public string IDGed { get; set; }

        public string FotoBase64 { get; set; }
        #endregion

        public Rescisao Rescisao { get; set; } 

        #region Métodos para conversão de data
        public DateTime NascimentoConversao()
        {
            try
            {
                return Convert.ToDateTime(Nascimento);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        public DateTime? DataAlteracaoCPFConversao()
        {
            try
            {
                return Convert.ToDateTime(DataAlteracaoCPF);
            }
            catch
            {
                return null;
            }
        }

        public TimeSpan? HorarioInicioTrabalhoConversao()
        {
            try
            {
                return TimeSpan.Parse(HorarioInicioTrabalho);
            }
            catch
            {
                return null;
            }
        }

        public TimeSpan? HorarioTerminoTrabalhoConversao()
        {
            try
            {
                return TimeSpan.Parse(HorarioTerminoTrabalho);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        public string Protocolo { get; set; }

        #region Informações Acadêmicas
        public List<InformacoesAcademica> InformacoesAcademica { get; set; }
        #endregion
    }
}
