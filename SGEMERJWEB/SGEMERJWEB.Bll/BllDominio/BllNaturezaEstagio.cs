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
    public class BllNaturezaEstagio
    {
        private readonly DalNaturezaEstagio dalNaturezaEstagio;

        public BllNaturezaEstagio(string codusu)
        {
            dalNaturezaEstagio = new DalNaturezaEstagio(codusu);
        }
        
        public NaturezaEstagio Obter(int id)
        {
            try
            {
                return dalNaturezaEstagio.Obter(id);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<NaturezaEstagio> ObterLista()
        {
            try
            {
                return dalNaturezaEstagio.ObterLista();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
