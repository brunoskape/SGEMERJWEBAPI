using SGEMERJWEB.Dal;
using SGEMERJWEB.Entidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGEMERJWEB.Bll
{
    public class BllCEP
    {
        private DalCEP dalCEP;

        public BllCEP()
        {
            dalCEP = new DalCEP(null);
        }

        public Endereco ObterEnderecoPeloCEP(string cep)
        {

            try
            {
                var endereco = dalCEP.ObterEnderecoPeloCEP(cep);

                if (endereco != null)
                {
                    var tipoLogradouro = BllDominioFachada.BllTipoLogradouro().Obter(endereco.TipoLogradouro.Descricao);
                    if (tipoLogradouro != null)
                    {
                        endereco.TipoLogradouro.Id = tipoLogradouro.Id;
                    }

                }

                return endereco;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
