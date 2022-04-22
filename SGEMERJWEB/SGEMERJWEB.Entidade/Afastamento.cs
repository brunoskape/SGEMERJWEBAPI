using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;
using System;

namespace SGEMERJWEB.Entidade
{
    public class Afastamento : EntidadeBase
    {
        public string DataInicio { get; set; }
        public string DataTermino { get; set; }
        public string ObservacaoAfastamento { get; set; }
        public MotivoAfastamento MotivoAfastamento { get; set; }

        #region Métodos para conversão de data
        public DateTime? DataInicioConversao()
        {
            try
            {
                return Convert.ToDateTime(DataInicio);
            }
            catch
            {
                return null;
            }
        }
        public DateTime? DataTerminoConversao()
        {
            try
            {
                return Convert.ToDateTime(DataTermino);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        public Afastamento()
        {
            MotivoAfastamento = new MotivoAfastamento();
        }
    }
}
