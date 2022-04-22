using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;


namespace SGEMERJWEB.Entidade.DTO
{
    public class ColaboradorGridDTO
    {
        public ColaboradorGridDTO()
        {
            Tipo = new TipoColaborador();
            Rescisao = new Rescisao();
        }

        public int Id { get; set; }

        public int IdDoTipo { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public TipoColaborador Tipo { get; set; }

        public bool Ativo { get; set; }

        public long Linha { get; set; }

        public Rescisao  Rescisao{get;set;} 
    }
}
