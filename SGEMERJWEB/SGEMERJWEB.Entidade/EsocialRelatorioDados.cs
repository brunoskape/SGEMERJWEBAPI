using Newtonsoft.Json;
using System.Collections.Generic;


namespace SGEMERJWEB.Entidade
{
    public class EsocialRelatorioDados
    {
        public string Relatorio { get; set; }
        public string Referencia { get; set; }
        public string Inscricao { get; set; }
        public string Evento { get; set; }
        public string Fase { get; set; }
        public string Status { get; set; }
        public List<Registro> Registros {get; set;}
    }

    public class Registro
    {
        public string Identificador { get; set; }
        public string ChaveValor { get; set; }
        public string Fase { get; set; }
        public string StatusDescricao { get; set; }
        public List<AtributoValor> Dados { get; set; }
        public List<Erros> Erros { get; set; }
        public string NrReciboPortal { get; set; }
    }

    public class AtributoValor
    {
        public string Atributo { get; set; }
        public string Valor { get; set; }
    }

    public class EsocialDadosRelatorio
    {
        public EsocialDadosRelatorio()
        {
            colaborador = new Colaborador();
        }

        public string identificadorMPS { get; set; }
        public Colaborador colaborador { get; set; }
        public string competencia { get; set; }
        public string evento { get; set; }
        public string retornoMpsEsocial { get; set; }
        public string status { get; set; }
        public string reciboEsocial { get; set; }
    }


    public class EsocialRelatorioDadosErros
    {
        public List<Registro> Registros { get; set; }
    }

    public class Erros
    {
        public string Codigo { get; set; }
        public string Mensagem { get; set; }
    }


}
