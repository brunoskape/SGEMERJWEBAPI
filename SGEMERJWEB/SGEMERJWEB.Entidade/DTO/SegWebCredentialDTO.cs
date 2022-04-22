using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGEMERJWEB.Entidade.DTO
{
    public class SegWebCredentialDTO
    {
        [JsonProperty("SIGLASISTEMA")]
        public string SiglaSistema;

        [JsonProperty("CHAVE")]
        public string SessionId { get; set; }

        [JsonProperty("CODORGAO")]
        public string CodigoOrganizacao { get; set; }

        public SegWebCredentialDTO(string _siglaSistema, string _sessionId, string _codigoOrganizacao)
        {
            SiglaSistema = _siglaSistema;
            SessionId = _sessionId;
            CodigoOrganizacao = _codigoOrganizacao;
        }
    }
}