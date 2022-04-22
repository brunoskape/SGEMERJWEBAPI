using SGEMERJWEB.Entidade.Dominio;
using System;

namespace SGEMERJWEB.Entidade
{
    public class DadoTSVE : EntidadeBase
    {
        public CadastramentoInicial CadastramentoInicial { get; set; }
        public string Matricula{ get; set; }
        public CategoriaTrabalhador CategoriaTrabalhador{ get; set; }
        public string DataInicio { get; set; }
        public string NumeroProcesso { get; set; }
        public string Cargo { get; set; }

        public CBOCargo CBOCargo { get; set; }
        public NaturezaAtividade NaturezaAtividade{ get; set; }

        public DadoTSVE()
        {
            CadastramentoInicial = new CadastramentoInicial();

            CategoriaTrabalhador = new CategoriaTrabalhador();

            NaturezaAtividade = new NaturezaAtividade();

            CBOCargo = new CBOCargo();
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

    }
}
