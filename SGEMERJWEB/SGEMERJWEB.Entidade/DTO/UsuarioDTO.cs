using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGEMERJWEB.Entidade.DTO
{
    public class UsuarioDTO
    {
        public int? IDPessoa { get; set; }
        public string codUsu { get; set; }
        public string Matricula { get; set; }
        public int? IDFunc { get; set; }
        public int? idUsu { get; set; }
        public string Nome { get; set; }
        public List<AutorizacaoDTO> Autorizacoes { get; set; }
        public TokenDTO Token { get; set; }

        public UsuarioDTO()
        {
            Token = new TokenDTO();
            Autorizacoes = new List<AutorizacaoDTO>();
        }
    }

    public class AutorizacaoDTO
    {
        public string Janela { get; set; }
        public string IndAutorizadoCertDig { get; set; }
        public string UrlCertDig { get; set; }
        public string ObjFunc { get; set; }
        public string SiglaFunc { get; set; }
        public string IndAutorizado { get; set; }
    }

    public class TokenDTO
    {
        public string TokenJWT { get; set; }
        public DateTime Expiration { get; set; }
        public void Parse(string _tokenJWT, DateTime _expiration)
        {
            TokenJWT = _tokenJWT;
            Expiration = _expiration;        
        }
    }
}


