using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;
using System;

namespace SGEMERJWEB.Entidade
{
    public class DadoProfissional : EntidadeBase
    {
        public int Ano { get; set; }
        public string Empresa { get; set; }
        public string CargoOcupado { get; set; }
        public string DataInicioPeriodo { get; set; }
        public string DataFimPeriodo { get; set; }

        public DateTime? DataInicioPeriodoConversao()
        {
            try
            {
                return Convert.ToDateTime(DataInicioPeriodo);
            }
            catch
            {
                return null;
            }
        }

        public DateTime? DataFimPeriodoConversao()
        {
            try
            {
                return Convert.ToDateTime(DataFimPeriodo);
            }
            catch
            {
                return null;
            }
        }
    }
}
