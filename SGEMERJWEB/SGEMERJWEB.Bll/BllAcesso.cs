using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SGEMERJWEB.Entidade.DTO;
using System;
using System.Net.Http;
using System.Text;

namespace SGEMERJWEB.Bll
{
    public class BllAcesso
    {
        private readonly string UrlValidaPermissao;
        private readonly string SiglaDoSistema;
        private readonly HttpClient HttpClient;
        private readonly string Ambiente;
        public BllAcesso(string _siglaSistema, string _urlValidaPermissao, string _ambiente)
        {
            HttpClient = new HttpClient();
            UrlValidaPermissao = _urlValidaPermissao;
            SiglaDoSistema = _siglaSistema;
            Ambiente = _ambiente;
        }

        public UsuarioDTO ObterUsuarioComSessionID(string SEGSESSIONID, string CODORG)
        {
            try
            {

                //if (CODORG == "9876543210" && (Ambiente == "H" || Ambiente=="D"))
                //{
                //    var usuarioFake = new UsuarioDTO
                //    {
                //        Nome = "USUÁRIO DE DESENVOLVIMENTO"
                //    };

                //    AutorizacaoDTO autorizacao = new AutorizacaoDTO
                //    {
                //        IndAutorizado = "S",
                //        SiglaFunc = "PRINC"
                //    };

                //    usuarioFake.Autorizacoes.Add(autorizacao);

                //    autorizacao = new AutorizacaoDTO
                //    {
                //        IndAutorizado = "S",
                //        SiglaFunc = "CAD_COLAB"
                //    };

                //    usuarioFake.Autorizacoes.Add(autorizacao);

                //    autorizacao = new AutorizacaoDTO
                //    {
                //        IndAutorizado = "N",
                //        SiglaFunc = "CAD_SETOR"
                //    };

                //    usuarioFake.Autorizacoes.Add(autorizacao);

                //    return usuarioFake;

                //}

                var segwebCredencial = new SegWebCredentialDTO(SiglaDoSistema, SEGSESSIONID, CODORG);
                string json = JsonConvert.SerializeObject(segwebCredencial);

                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = HttpClient.PostAsync(this.UrlValidaPermissao, data);

                string resposta = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                JObject jsonContent = JObject.Parse(resposta);
                var usuario = JsonConvert.DeserializeObject<UsuarioDTO>(jsonContent["IdentificacaoUsuario"].ToString());

                if (usuario != null)
                {
                    if (!string.IsNullOrEmpty(usuario.Nome) && usuario.Autorizacoes.Count > 0)
                    {
                        int qtdAutorizados = usuario.Autorizacoes.FindAll(x => x.IndAutorizado == "S").Count;
                        if (qtdAutorizados > 0)
                            return usuario;
                    }
                }
                return null;
            }
            catch
            {
                throw new Exception("Erro ao recuperar os dados do usuário");
            }

        }


    }
}
