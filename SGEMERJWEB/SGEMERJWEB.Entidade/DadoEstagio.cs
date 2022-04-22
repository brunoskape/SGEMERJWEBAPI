using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;
using System;
using System.Collections.Generic;

namespace SGEMERJWEB.Entidade
{
    public class DadoEstagio : EntidadeBase
    {
        #region  Dados do estágio
        public NaturezaEstagio NaturezaEstagio { get; set; }
        public NivelEstagio NivelEstagio { get; set; }
        public Setor SetorEstagio { get; set; }
        public string ApoliceSeguro { get; set; }
        public string AreaAtuacao { get; set; }
        public string DataPrevistaTermino { get; set; }
        public string DataInicio { get; set; }
        public string HorarioInicio { get; set; }
        public string HorarioTermino { get; set; }
        public bool EHorarioDinamico { get; set; }
        public string CPFSupervisor { get; set; }
        public string NomeSupervisor { get; set; }
        #endregion

        #region Instituição de Ensino
        public string CNPJInstEnsino { get; set; }
        public string RazaoSocialInstEnsino { get; set; }
        public Endereco EnderecoInstEnsino { get; set; }
        #endregion

        #region Agente de Integração
        public string CNPJAgenteInt { get; set; }
        public string RazaoSocialAgenteInt { get; set; }
        #endregion

        public DadoEstagio()
        {
            NaturezaEstagio = new NaturezaEstagio();
            NivelEstagio = new NivelEstagio();
            SetorEstagio = new Setor();

            EnderecoInstEnsino = new Endereco();
        }

        #region Métodos para conversão de data e Hora
        public DateTime DataPrevistaTerminoConversao()
        {
            try
            {
                return Convert.ToDateTime(DataPrevistaTermino);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public DateTime DataInicioConversao()
        {
            try
            {
                return Convert.ToDateTime(DataInicio);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public TimeSpan? HorarioInicioConversao()
        {
            try
            {
                return TimeSpan.Parse(HorarioInicio);
            }
            catch
            {
                return null;
            }
        }

        public TimeSpan? HorarioTerminoConversao()
        {
            try
            {
                return TimeSpan.Parse(HorarioTermino);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
