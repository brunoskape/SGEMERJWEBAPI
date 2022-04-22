using Newtonsoft.Json;
using System;

namespace SGEMERJWEB.WebAPI.Model
{
    public class RetornoModel
    {
        public int? TotalPagina { get; set; }

        public int? TotalRegistro { get; set; }

        public int? Pagina { get; set; }

        public Object Data { get; set; }

        public string Status { get; set; }

        public string Mensagem { get; set; }



        static public RetornoModel CriarRetorno(int? _totalPagina, int? _totalRegistro, int? _pagina, Object _data, string _status, string _mensagem="")
        {
            var retorno = new RetornoModel
            {
                TotalPagina = _totalPagina,
                TotalRegistro = _totalRegistro,
                Pagina = _pagina,
                Data = _data,
                Status = _status,
                Mensagem = _mensagem
            };
            return retorno;
        }
    }
}