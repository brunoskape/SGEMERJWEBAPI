using Newtonsoft.Json;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;
using System.Collections.Generic;

namespace SGEMERJWEB.Entidade
{
    public class Deficiencia : EntidadeBase
    {
        public List<ClassificacaoDeficiencia> ClassificacaoDeficiencias { get; set; }
        public bool EhPortadorDeNecessidadeEspecial { get; set; }
        public bool EhReabilitadoReadaptado { get; set; }
        public string ObservacaoDeficiencia { get; set; }

        public Deficiencia()
        {
            ClassificacaoDeficiencias = new List<ClassificacaoDeficiencia>();
        }
    }
}
