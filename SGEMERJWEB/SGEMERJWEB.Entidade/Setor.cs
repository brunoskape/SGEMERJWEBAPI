using Newtonsoft.Json;

namespace SGEMERJWEB.Entidade
{
    public class Setor : EntidadeBase
    {
        public string Descricao { get; set; }

        public string Sigla { get; set; }
        
        public bool Ativo { get; set; }
    }
}
