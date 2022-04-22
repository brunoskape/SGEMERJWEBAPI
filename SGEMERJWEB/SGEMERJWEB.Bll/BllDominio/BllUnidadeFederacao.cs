using SGEMERJWEB.Dal;
using SGEMERJWEB.Entidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGEMERJWEB.Entidade.Dominio;

namespace SGEMERJWEB.Bll
{
    public class BllUnidadeFederacao
    {
        private readonly DalUnidadeFederacao dalUnidadeFederacao;

        public BllUnidadeFederacao(string codusu=null)
        {
            dalUnidadeFederacao = new DalUnidadeFederacao(codusu);
        }
        
        public UnidadeFederacao Obter(string id)
        {
            try
            {
                return dalUnidadeFederacao.Obter(id);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<UnidadeFederacao> ObterLista()
        {
            try
            {
                return dalUnidadeFederacao.ObterLista();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
