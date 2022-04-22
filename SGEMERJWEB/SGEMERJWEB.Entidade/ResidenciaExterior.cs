using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;
using System;

namespace SGEMERJWEB.Entidade
{
    public class ResidenciaExterior : EntidadeBase
    {
        public Pais PaisResidencia { get; set; }
        public string CodigoPostal { get; set; }
        public string Descricao { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }

        public ResidenciaExterior()
        {
            PaisResidencia = new Pais();
        }

    }
}
