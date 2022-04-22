using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;
using System;

namespace SGEMERJWEB.Entidade
{
    public class Rescisao : EntidadeBase
    {
        public Rescisao()
        {
            MotivoRescisao = new MotivoRescisao();
        }
        public string DataTermino { get; set; }
        public string NumeroRecibo { get; set; }
        public string NumeroProcessoTrabalhista { get; set; }
        public bool IndGuia { get; set; }
        public MotivoRescisao MotivoRescisao { get; set; }
        public string DataFimQuarentena { get; set; }
    }
}
