using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;

namespace SGEMERJWEB.Entidade
{
    public class EmailPessoa : EntidadeBase
    {
        public string Email { get; set; }
        public TipoContatoEnum TipoContato { get; set; }

        public EmailPessoa()
        {
            TipoContato = TipoContatoEnum.Principal;
        }
    }
}
