using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SGEMERJWEB.Entidade;

namespace SGEMERJWEB.Dal
{
    public class DalSetor : DalBase
    {
        public DalSetor(string codusu) : base(codusu)
        {
        }
        /// <summary>
        /// Inclui ou altera um Setor
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        public string GravarSetor(Setor setor)
        {
            var ret = "";

            try
            {
                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }

                trans = sd.BeginTransaction();


                if (setor.Id == 0)
                {
                    sd.ExecutaProc("PKG_SETOR.SP_INCLUIR_SETOR", setor.Descricao, setor.Sigla, setor.Ativo ? "S" : "N");
                    ret = "INCLUIDO";
                }
                else
                {
                    sd.ExecutaProc("PKG_SETOR.SP_ALTERAR_SETOR", setor.Id, setor.Descricao, setor.Sigla, setor.Ativo ? "S" : "N");
                    ret = "ALTERADO";
                }

                trans.Commit();
            }
            catch (ServicoDadosODPNETM.ServicoDadosException sdEx)
            {
                trans.Rollback();
                throw new Exception(sdEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                sd.Connection.Close();
            }

            return ret;
        }


        /// <summary>
        /// Retorna lista de Setores para os filtros informados.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        public IEnumerable<Setor> RetornarSetores(Setor setor)
        {
            var lstSetor = new List<Setor>();

            try
            {
                var dtSetores = sd.ExecutaProcDS("PKG_SETOR.SP_RETORNAR_SETOR", null, setor.Descricao, setor.Sigla, sd.CriaRefCursor()).Tables[0];

                lstSetor = (from DataRow dr in dtSetores.Rows
                            select new Setor
                            {
                                Id = int.Parse(dr["ID"].ToString()),
                                Descricao = dr["DESCRICAO"].ToString(),
                                Sigla = dr["SIGLA"].ToString(),
                                Ativo = dr["ATIVO"].ToString() == "S" ? true : false
                            }).ToList();

                return lstSetor;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarSetores] " + ex.ToString());
            }
            finally
            {
                lstSetor = null;
                sd.Connection.Close();
            }
        }


        /// <summary>
        /// Retorna lista de Setores ativos.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        public IEnumerable<Setor> RetornarSetoresAtivos()
        {
            var lstSetor = new List<Setor>();

            try
            {
                var dtSetores = sd.ExecutaProcDS("PKG_COMBOS.SP_SETORATIVO", sd.CriaRefCursor()).Tables[0];

                lstSetor = (from DataRow dr in dtSetores.Rows
                            select new Setor
                            {
                                Id = int.Parse(dr["ID"].ToString()),
                                Descricao = dr["DESCRICAO"].ToString(),
                                Sigla = dr["SIGLA"].ToString(),
                                Ativo = dr["ATIVO"].ToString() == "S" ? true : false
                            }).ToList();

                return lstSetor;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarSetoresAtivos] " + ex.ToString());
            }
            finally
            {
                lstSetor = null;
                sd.Connection.Close();
            }
        }

        /// <summary>
        /// Retorna o setor com base no id informado.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        public Setor RetornarSetor(int id)
        {
            var setor = new Setor();

            try
            {
                if (id == 0)
                    return null;

                var dtSetores = sd.ExecutaProcDS("PKG_SETOR.SP_RETORNAR_SETOR", id, null, null, sd.CriaRefCursor()).Tables[0];

                setor = (from DataRow dr in dtSetores.Rows
                         select new Setor
                         {
                             Id = int.Parse(dr["ID"].ToString()),
                             Descricao = dr["DESCRICAO"].ToString(),
                             Sigla = dr["SIGLA"].ToString(),
                             Ativo = dr["ATIVO"].ToString() == "S" ? true : false
                         }).FirstOrDefault();

                return setor;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarSetor] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }

        /// <summary>
        /// Retorna o setor com base no id informado.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        public bool SiglaExiste(int id, string sigla)
        {
            try
            {
                var dtSetores = sd.ExecutaProcDS("PKG_SETOR.sp_sigla_existe", id, sigla, sd.CriaRefCursor()).Tables[0];

                if (dtSetores.Rows.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("[SiglaExiste] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }
    }
}
