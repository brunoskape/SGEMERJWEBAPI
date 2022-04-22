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

    public class DalUnidadeFederacao: DalBase
    {
        public DalUnidadeFederacao(string codusu) : base(codusu)
        {
        }
        public List<UnidadeFederacao> ObterLista()
        {
            try
            {
                List<UnidadeFederacao> lista = BuscarPorFiltro(null);
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

        public UnidadeFederacao Obter(string id)
        {
            try
            {
                List<UnidadeFederacao> lista = BuscarPorFiltro(id.ToUpper());
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


        private List<UnidadeFederacao> BuscarPorFiltro(string id)
        {
            try
            {
                var lista = new List<UnidadeFederacao>();

                var dtLista = sd.ExecutaProcDS("PKG_COMBOS.SP_UNIDADEFEDERACAO", id, sd.CriaRefCursor()).Tables[0];

                lista = (from DataRow dr in dtLista.Rows
                         select new UnidadeFederacao
                         {
                             Id = dr["ID"].ToString(),
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
