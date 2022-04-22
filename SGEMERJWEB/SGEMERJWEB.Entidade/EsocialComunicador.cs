using System.Collections.Generic;

namespace SGEMERJWEB.Entidade
{
    public class EsocialComunicador : EntidadeBase
    {
        public int Id_Colaborador { get; set; }
        public int Id_Tipo_Colaborador { get; set; }
        public string Evento { get; set; }
        public string DataCriacao { get; set; }
        public string DataEnvio { get; set; }
        public string Status { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        
        public int CodigoStatus { get; set; }

        public string Recibo { get; set; }
    }

    public class EventoEsocial
    {
        public string tabela { get; set; }
        public int tipoArquivo { get; set; }
        public int tipoInscricao { get; set; }
        public string nroInscricao { get; set; }
        public string anoMesReferencia { get; set; }
        public string indice { get; set; }
        public string id { get; set; }
        public string identificador { get; set; }

        public List<AtributoEvento> atributos { get; set; }

        public List<ListaGenerica> listas { get; set; }
    }

    public class EventoEsocial2306
    {
        public string tabela { get; set; }
        public int tipoArquivo { get; set; }
        public int tipoInscricao { get; set; }
        public string nroInscricao { get; set; }
        public string anoMesReferencia { get; set; }
        public string id { get; set; }
        public string identificador { get; set; }

        public List<AtributoEvento> atributos { get; set; }
    }

    public class EventoEsocial2399
    {
        public string tabela { get; set; }
        public int tipoArquivo { get; set; }
        public int tipoInscricao { get; set; }
        public string nroInscricao { get; set; }
        public string anoMesReferencia { get; set; }
        public string id { get; set; }
        public string identificador { get; set; }

        public List<AtributoEvento> atributos { get; set; }
    }

    public class EventoEsocial2205
    {
        public string tabela { get; set; }
        public int tipoArquivo { get; set; }
        public int tipoInscricao { get; set; }
        public string nroInscricao { get; set; }
        public string anoMesReferencia { get; set; }
        public string indice { get; set; }
        public string id { get; set; }
        public string identificador { get; set; }

        public List<AtributoEvento> atributos { get; set; }

        public List<ListaGenerica> listas { get; set; }
    }

    public class AtributoEvento
    {
        public string atributo { get; set; }
        public string valor { get; set; }
    }

    public class ListaGenerica
    {
        public string tabela { get; set; }
        public List<AtributoEvento> atributos { get; set; }
    }

    public class inconcistenciaAtributo
    {
        public string campo { get; set; }
        public string valor { get; set; }
        public string mensagem { get; set; }
    }

    public class inconcistenciaHeader
    {
        public string campo { get; set; }
        public string valor { get; set; }
        public string mensagem { get; set; }
    }

    public class EsocialRelatorio
    {
        public List<inconcistenciaHeader> inconsistenciasHeader { get; set; }
        public List<inconcistenciaAtributo> inconsistenciasAtributo { get; set; }
        public string id { get => identificador.Split(';')[3]; }
        public string identificador { get; set; }
        public string indice { get; set; }
        public string tpInscricao { get; set; }
        public string nroInscricao { get; set; }
        public string anoMes { get; set; }
        public string tabela { get; set; }
        public string tipoArquivo { get; set; }
        public string statusInconsistenciaHeader { get; set; }
        public string statusInconsistenciaAtributo { get; set; }

        public int statusInconsistenciaRepetido { get; set; }
        public string statusInconsistenciaMsg { get; set; }
    }

    public class RetornoMensagem
    {
        public bool sucesso { get; set; }
        public string mensagem { get; set; }
    }

    public class comunicadorInconsistencia
    {
        public string IdComunicador { get; set; }
        public string Referencia { get; set; }
        public string Campo { get; set; }
        public string Valor { get; set; }
        public string Mensagem { get; set; }
        public string DataEnvio { get; set; }
        public string Aceito { get; set; }
    }

    public class AtributoEvento2306
    {
        public string TipoInscricao { get; set; }
        public string CPF { get; set; }
        public string DataAlteracao { get; set; }
        public string CodigoCategoria { get; set; }
        public string NomeCargo { get; set; }
        public string CBOCargo { get; set; }
    }

    public class ErrosEsocial
    {
        public string Identificador { get; set; }
        public string CodigoErro { get; set; }
        public string Mensagem { get; set; }
    }

}
