using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.Entidade.Dominio;
using System.Reflection;

namespace SGEMERJWEB.Dal
{
    public class DalDominioBase<T>: DalBase where T:DominioBase
    {
        private readonly string Procedure = "";
        public DalDominioBase(string codusu, string procedure):base(codusu)
        {
            Procedure = procedure;            
        }

        public List<T> ObterLista()
        {
            try
            {
                List<T> lista = BuscarPorFiltro(null,null);
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

        public T Obter(int id)
        {
            try
            {
                List<T> lista = BuscarPorFiltro(id,null);
                if (lista != null)
                    if (lista.Count > 0)
                        return lista.FirstOrDefault();
                return default;
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

        public T Obter(string descricao)
        {
            try
            {
                List<T> lista = BuscarPorFiltro(null, descricao);
                if (lista != null)
                    if (lista.Count > 0)
                        return lista.FirstOrDefault();
                return default;
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


        private List<T> BuscarPorFiltro(int? id, string descricao)
        {
            try
            {
                var lista = new List<T>();
                string idAux = id == null ? "" : id.ToString();
                DataTable dtLista;

                dtLista = sd.ExecutaProcDS(Procedure, id, descricao, sd.CriaRefCursor()).Tables[0];

                lista = (from DataRow dr in dtLista.Rows
                         select (T)GetObject(int.Parse(dr["ID"].ToString()), dr["DESCRICAO"].ToString())
                         ).ToList();

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private T GetObject(int id, string descricao)
        {
            T objeto = (T)Activator.CreateInstance(typeof(T));

            PropertyInfo propertyInfo = objeto.GetType().GetProperty("Id");
            propertyInfo.SetValue(objeto, Convert.ChangeType(id, propertyInfo.PropertyType), null);

            propertyInfo = objeto.GetType().GetProperty("Descricao");
            propertyInfo.SetValue(objeto, Convert.ChangeType(descricao, propertyInfo.PropertyType), null);

            return objeto;
        }
    }
}
