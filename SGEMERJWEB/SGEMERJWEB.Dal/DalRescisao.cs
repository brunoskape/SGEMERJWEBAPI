using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.Entidade.DTO;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.Enum;

namespace SGEMERJWEB.Dal
{
    public class DalRescisao : DalBase
    {
        private readonly string codUsuario;
        private readonly string idUsuario;

        public DalRescisao(string codusu) : base(codusu)
        {
            codUsuario = codusu;
        }

        public DalRescisao(string codusu, string idusu) : base(codusu)
        {
            codUsuario = codusu;
            idUsuario = idusu;
        }

        public Colaborador ObterRescisaoPorId(string idDoTipo)
        {
            try
            {
                Colaborador c = new Colaborador();
                DataTable dt;
                dt = sd.ExecutaProcDS("pkg_colaborador_recisao.consultar", sd.CriaRefCursor("c"),idDoTipo).Tables[0];

                foreach (DataRow linha in dt.Rows)
                {
                    if (linha["idRescisao"].ToString() != "")
                    {
                        c.Rescisao.Id = int.Parse(linha["idRescisao"].ToString()); //ID_RESCISAO
                        c.Rescisao.DataTermino = string.Format("{0:d}", linha["dt_recisao"]);
                        c.Rescisao.NumeroProcessoTrabalhista = linha["nr_proc_trab"].ToString();
                        //c.Rescisao.IndGuia = linha["ind_guia"].ToString();
                        c.Rescisao.IndGuia = linha["ind_guia"].ToString() == "1" ? true : false ;
                        c.Rescisao.MotivoRescisao.Id = int.Parse(linha["id_motivo_recisao"].ToString() == "" ? "0" : linha["id_motivo_recisao"].ToString());//ID_MOTIVO_RECISAO
                        //c.Rescisao.MotivoRescisao.Descricao = linha["descricao"].ToString();
                        c.Rescisao.DataFimQuarentena = string.Format("{0:d}", linha["dt_fim_quarentena"]);
                    }
                    c.Id = int.Parse(linha["idColaborador"].ToString()); //ID_COLABORADOR"
                    c.IdDoTipo = int.Parse(linha["idColaboradorTipo"].ToString());//ID_COLABORADOR_TIPO
                    c.CPF = linha["cpf"].ToString();
                    c.CPFAntigo = linha["cpf_antigo"].ToString();
                    c.Nome = linha["nome"].ToString();
                    c.DadoTSVE.Matricula = linha["matricula"].ToString();
                    c.TipoColaborador.Id = int.Parse(linha["id_tipo_colaborador"].ToString());//ID_TIPO_COLABORADOR
                    c.Protocolo = linha["nr_recibo"].ToString();
                }

                return c;
            }
            catch (Exception ex)
            {
                throw new Exception("[ObterRescisaoPorId] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }

        }

        public string incluir(Colaborador objeto)
        {
            try
            {
                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }
                trans = sd.BeginTransaction();

                var V_IndGuia = objeto.Rescisao.IndGuia == true ? "1" : "";

                var V_motResc = objeto.Rescisao.MotivoRescisao.Id == 0 ? "" : objeto.Rescisao.MotivoRescisao.Id.ToString();


                sd.ExecutaProc("pkg_colaborador_recisao.incluir", 
                    objeto.IdDoTipo,
                    objeto.Rescisao.DataTermino,
                    objeto.Protocolo,
                    objeto.Rescisao.NumeroProcessoTrabalhista,
                    V_IndGuia,
                    V_motResc,
                    objeto.Rescisao.DataFimQuarentena);

                sd.ExecutaProc("pkg_esocial.gravar_evento2399",
                                objeto.Id,
                                objeto.TipoColaborador.Id,
                                objeto.Protocolo);

                trans.Commit();

                return "INCLUIDO";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[incluir_recisao] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }

        }

        public string alterar(Colaborador objeto)
        {
            try
            {
                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }
                trans = sd.BeginTransaction();

                var V_IndGuia = objeto.Rescisao.IndGuia == true ? "1" : "";
                var V_motResc = objeto.Rescisao.MotivoRescisao.Id == 0 ? "" : objeto.Rescisao.MotivoRescisao.Id.ToString();

                sd.ExecutaProc("pkg_colaborador_recisao.alterar",
                    objeto.Rescisao.Id,
                    objeto.Rescisao.DataTermino,
                    objeto.Protocolo,
                    objeto.Rescisao.NumeroProcessoTrabalhista,
                    V_IndGuia,
                    V_motResc,
                    objeto.Rescisao.DataFimQuarentena);

                sd.ExecutaProc("pkg_esocial.gravar_evento2399",
                                objeto.Id,
                                objeto.TipoColaborador.Id,
                                objeto.Protocolo);

                trans.Commit();

                return "ALTERADO";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[alterar_recisao] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }

    }
}
