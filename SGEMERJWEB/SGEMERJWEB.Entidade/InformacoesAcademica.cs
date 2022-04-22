using SGEMERJWEB.Entidade.Dominio;

namespace SGEMERJWEB.Entidade
{
    public class InformacoesAcademica : EntidadeBase
    {
        public string Curso { get; set; }

        public Situacao Situacao { get; set; }

        public string PrevisaoConclusao { get; set; }

        public InformacoesAcademica()
        {
            Situacao = new Situacao();
        }

        protected int? TratarZeroComoNulo(int valor)
        {
            if (valor > 0)
                return valor;

            return null;
        }

    }
}
