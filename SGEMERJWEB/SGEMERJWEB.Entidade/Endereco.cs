using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;

namespace SGEMERJWEB.Entidade
{
    public class Endereco : EntidadeBase
    {
        public string CEP { get; set; }
        public TipoLogradouro TipoLogradouro { get; set; }
        public string NomeLogradouro { get; set; }
        public string NumeroLogradouro { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public UnidadeFederacao UF { get; set; }
        public TipoEnderecoEnum Tipo { get; set; }

        public Endereco()
        {
            Tipo = TipoEnderecoEnum.Principal;
            TipoLogradouro = new TipoLogradouro();
            UF = new UnidadeFederacao();
        }
    }
}
