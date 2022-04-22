using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;
using System;

namespace SGEMERJWEB.Entidade
{
    public class Dependente : EntidadeBase
    {
        public int idDependenteAux { get; set; }
        public TipoDependente TipoDependente { get; set; }
        public string Nome { get; set; }
        public string Nascimento { get; set; }
        public string CPF { get; set; }
        public bool TemIRRF { get; set; }
        public bool TemSalarioFamilia { get; set; }
        public bool TemIncapacidadeFisicaMental { get; set; }

        public Dependente()
        {            
            TipoDependente = new TipoDependente();         
        }

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

    }
}
