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

    public class DalCBO: DalBase
    {
        public DalCBO(string codusu) : base(codusu)
        {
        }
        public List<CBOCargo> ObterLista()
        {
            try
            {
                List<CBOCargo> lista = BuscarPorFiltro(null);
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

        public CBOCargo Obter(int id)
        {
            try
            {
                List<CBOCargo> lista = BuscarPorFiltro(id);
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


        private List<CBOCargo> BuscarPorFiltro(int? id)
        {
            try
            {
                var lista = new List<CBOCargo>();

                var dtLista = sd.ExecutaProcDS("PKG_COMBOS.SP_CBOCARGO", id, sd.CriaRefCursor()).Tables[0];

                lista = (from DataRow dr in dtLista.Rows
                         select new CBOCargo
                         {
                             Id = int.Parse( dr["ID"].ToString()),
                             Codigo = dr["CODIGO"].ToString().ToUpper(),
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
