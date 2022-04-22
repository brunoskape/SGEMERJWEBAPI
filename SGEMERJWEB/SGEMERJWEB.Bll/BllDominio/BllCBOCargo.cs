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
    public class BllCBOCargo
    {
        private readonly DalCBO dalCBOCargo;

        public BllCBOCargo(string codusu)
        {
            dalCBOCargo = new DalCBO(codusu);
        }
        
        public CBOCargo Obter(int id)
        {
            try
            {
                return dalCBOCargo.Obter(id);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<CBOCargo> ObterLista()
        {
            try
            {
                return dalCBOCargo.ObterLista();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
