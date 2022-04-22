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

    public class DalNaturezaEstagio: DalBase
    {
        public DalNaturezaEstagio(string codusu) : base(codusu)
        {
        }

        public List<NaturezaEstagio> ObterLista()
        {
            try
            {
                List<NaturezaEstagio> lista = BuscarPorFiltro(null);
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

        public NaturezaEstagio Obter(int id)
        {
            try
            {
                List<NaturezaEstagio> lista = BuscarPorFiltro(id);
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


        private List<NaturezaEstagio> BuscarPorFiltro(int? id)
        {
            try
            {
                var lista = new List<NaturezaEstagio>();

                var dtLista = sd.ExecutaProcDS("PKG_COMBOS.sp_NaturezaEstagio", id, sd.CriaRefCursor()).Tables[0];

                lista = (from DataRow dr in dtLista.Rows
                         select new NaturezaEstagio
                         {
                             Id = int.Parse( dr["ID"].ToString()),
                             Sigla = dr["SIGLA"].ToString().ToUpper(),
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
