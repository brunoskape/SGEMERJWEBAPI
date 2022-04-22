using SGEMERJWEB.Dal;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.Entidade.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGEMERJWEB.Bll
{
    public class BllDominioBase<TDominioBase> where TDominioBase : DominioBase
    {

        private readonly DalDominioBase<TDominioBase> objetoDal;

        public BllDominioBase(string codusu, string procedure)
        {
            objetoDal = new DalDominioBase<TDominioBase>(codusu,procedure);
        }

        public TDominioBase Obter(int id)
        {
            try
            {
                return objetoDal.Obter(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public TDominioBase Obter(string descricao)
        {
            try
            {
                return objetoDal.Obter(descricao);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<TDominioBase> ObterLista()
        {
            try
            {
                return objetoDal.ObterLista();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
