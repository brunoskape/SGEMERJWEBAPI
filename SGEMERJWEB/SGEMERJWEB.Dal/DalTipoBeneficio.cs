using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SGEMERJWEB.Entidade;

namespace SGEMERJWEB.Dal
{
    public class DalTipoBeneficio : DalBase
    {
        public DalTipoBeneficio(string codusu) : base(codusu)
        {
        }
        /// <summary>
        /// Inclui ou altera um tipoBeneficio
        /// </summary>
        /// <param name="tipoBeneficio"></param>
        /// <returns></returns>
        public string GravarTipoBeneficio(TipoBeneficio tipoBeneficio)
        {
            var ret = "";

            try
            {
                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }

                trans = sd.BeginTransaction();


                if (tipoBeneficio.Id == 0)
                {
                    sd.ExecutaProc("PKG_TIPO_BENEFICIO.SP_INCLUIR_TIPO_BENEFICIO", tipoBeneficio.Descricao,  tipoBeneficio.Ativo ? "S" : "N");
                    ret = "INCLUIDO";
                }
                else
                {
                    sd.ExecutaProc("PKG_TIPO_BENEFICIO.SP_ALTERAR_TIPO_BENEFICIO", tipoBeneficio.Id, tipoBeneficio.Descricao, tipoBeneficio.Ativo ? "S" : "N");
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
        /// Retorna lista de tipoBeneficioes para os filtros informados.
        /// </summary>
        /// <param name="tipoBeneficio"></param>
        /// <returns></returns>
        public IEnumerable<TipoBeneficio> RetornarTipoBeneficios(TipoBeneficio tipoBeneficio)
        {
            var lsttipoBeneficio = new List<TipoBeneficio>();

            try
            {
                var dtTipoBeneficioes = sd.ExecutaProcDS("PKG_TIPO_BENEFICIO.SP_RETORNAR_TIPO_BENEFICIO", null, tipoBeneficio.Descricao,  sd.CriaRefCursor()).Tables[0];

                lsttipoBeneficio = (from DataRow dr in dtTipoBeneficioes.Rows
                            select new TipoBeneficio
                            {
                                Id = int.Parse(dr["ID"].ToString()),
                                Descricao = dr["DESCRICAO"].ToString(),
                                Ativo = dr["ATIVO"].ToString() == "S" ? true : false
                            }).ToList();

                return lsttipoBeneficio;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornartipoBeneficioes] " + ex.ToString());
            }
            finally
            {
                lsttipoBeneficio = null;
                sd.Connection.Close();
            }
        }


        /// <summary>
        /// Retorna lista de tipoBeneficioes ativos.
        /// </summary>
        /// <param name="tipoBeneficio"></param>
        /// <returns></returns>
        public IEnumerable<TipoBeneficio> RetornartipoBeneficiosAtivos()
        {
            var lsttipoBeneficio = new List<TipoBeneficio>();

            try
            {
                var dtTipoBeneficioes = sd.ExecutaProcDS("PKG_COMBOS.sp_TipoBeneficioAtivo", sd.CriaRefCursor()).Tables[0];

                lsttipoBeneficio = (from DataRow dr in dtTipoBeneficioes.Rows
                            select new TipoBeneficio
                            {
                                Id = int.Parse(dr["ID"].ToString()),
                                Descricao = dr["DESCRICAO"].ToString(),
                                Ativo = dr["ATIVO"].ToString() == "S" ? true : false
                            }).ToList();

                return lsttipoBeneficio;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornartipoBeneficioesAtivos] " + ex.ToString());
            }
            finally
            {
                lsttipoBeneficio = null;
                sd.Connection.Close();
            }
        }

        /// <summary>
        /// Retorna o tipoBeneficio com base no id informado.
        /// </summary>
        /// <param name="tipoBeneficio"></param>
        /// <returns></returns>
        public TipoBeneficio RetornarTipoBeneficio(int id)
        {
            var tipoBeneficio = new TipoBeneficio();

            try
            {
                if (id == 0)
                    return null;

                var dttipoBeneficioes = sd.ExecutaProcDS("PKG_TIPO_BENEFICIO.SP_RETORNAR_TIPO_BENEFICIO", id,  null, sd.CriaRefCursor()).Tables[0];

                tipoBeneficio = (from DataRow dr in dttipoBeneficioes.Rows
                         select new TipoBeneficio
                         {
                             Id = int.Parse(dr["ID"].ToString()),
                             Descricao = dr["DESCRICAO"].ToString(),
                             Ativo = dr["ATIVO"].ToString() == "S" ? true : false
                         }).FirstOrDefault();

                return tipoBeneficio;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornartipoBeneficio] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }

        /// <summary>
        /// Retorna o tipoBeneficio com base no id informado.
        /// </summary>
        /// <param name="tipoBeneficio"></param>
        /// <returns></returns>
        public bool BeneficioExiste(int id, string descricao)
        {
            try
            {
                var dtTipoBeneficioes = sd.ExecutaProcDS("PKG_TIPO_BENEFICIO.SP_BENEFICIO_EXISTE", id, descricao, sd.CriaRefCursor()).Tables[0];

                if (dtTipoBeneficioes.Rows.Count > 0)
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
