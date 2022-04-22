using Newtonsoft.Json;

namespace SGEMERJWEB.Entidade
{
    public class TipoBeneficio : EntidadeBase
    {
        public string Descricao { get; set; }
        
        public bool Ativo { get; set; }
    }
}
