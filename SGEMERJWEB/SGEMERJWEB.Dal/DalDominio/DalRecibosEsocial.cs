using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.Entidade.Dominio;

namespace SGEMERJWEB.Dal
{

    public class DalRecibosEsocial: DalBase
    {
        public DalRecibosEsocial(string codusu) : base(codusu)
        {
        }
        public List<RecibosEsocial> ObterLista(int id)
        {
            try
            {
                List<RecibosEsocial> lista = BuscarPorFiltro(id);
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarLista] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }

        public RecibosEsocial Obter(int id)
        {
            try
            {
                List<RecibosEsocial> lista = BuscarPorFiltro(id);
                if (lista != null)
                    if (lista.Count > 0)
                        return lista.FirstOrDefault();
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarObjeto] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }


        private List<RecibosEsocial> BuscarPorFiltro(int? id)
        {
            try
            {
                var lista = new List<RecibosEsocial>();

                var dtLista = sd.ExecutaProcDS("PKG_COMBOS.SP_RECIBOSESOCIAL", id, sd.CriaRefCursor()).Tables[0];

                lista = (from DataRow dr in dtLista.Rows
                         select new RecibosEsocial
                         {
                             Id = int.Parse( dr["ID"].ToString()),
                             Evento = dr["EVENTO"].ToString().ToUpper(),
                             Descricao = dr["DESCRICAO"].ToString().ToUpper()
                         }).ToList();

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
