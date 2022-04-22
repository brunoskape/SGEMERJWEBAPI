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
    public class BllRecibosEsocial
    {
        private readonly DalRecibosEsocial dalRecibosEsocial;

        public BllRecibosEsocial(string codusu)
        {
            dalRecibosEsocial = new DalRecibosEsocial(codusu);
        }
        
        public RecibosEsocial Obter(int id)
        {
            try
            {
                return dalRecibosEsocial.Obter(id);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<RecibosEsocial> ObterLista(int id)
        {
            try
            {
                return dalRecibosEsocial.ObterLista(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
