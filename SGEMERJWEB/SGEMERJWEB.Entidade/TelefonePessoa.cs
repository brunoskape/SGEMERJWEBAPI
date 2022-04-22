using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;

namespace SGEMERJWEB.Entidade
{
    public class TelefonePessoa : EntidadeBase
    {
        public int? DDD { get; set; }
        public int? Numero { get; set; }
        public TipoContatoEnum TipoContato { get; set; }

        public TelefonePessoa()
        {
            TipoContato = TipoContatoEnum.Principal;
        }
    }
}
