using SGEMERJWEB.Entidade.Relatorios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SGEMERJWEB.Dal.Relatorios
{
    public class DalRelFuncionariosAtivos : DalBase
    {
        DateTime dataAux;

        public DalRelFuncionariosAtivos(string codusu) : base(codusu)
        {
        }

        public List<FuncionariosAtivos> RetornarFuncionariosAtivos(int tipo, string dataInicio, string dataFim)
        {
            var listaFuncionariosAtivos = new List<FuncionariosAtivos>();

            try
            {
                var dtFuncionariosAtivos = sd.ExecutaProcDS("PKG_RELATORIOS.sp_consultaFuncionariosAtivos", tipo, dataInicio, dataFim, sd.CriaRefCursor()).Tables[0];

                listaFuncionariosAtivos = (from DataRow dr in dtFuncionariosAtivos.Rows
                                    select new FuncionariosAtivos
                                    {
                                        NomeCompleto = dr["Nome Completo"].ToString(),
                                        CPF = dr["CPF"].ToString(),
                                        Setor = dr["Setor"].ToString(),
                                        EmailPrincipal = dr["E-mail Principal"].ToString(),
                                        TelefonePrincipal = dr["Telefone Principal"].ToString(),
                                        Cargo = dr["Cargo"].ToString(),
                                        Matricula = dr["Matrícula"].ToString(),
                                        CBOCargo = dr["CBO Cargo"].ToString(),
                                        DataInicio = DateTime.TryParse(dr["Data de Início"].ToString(), out dataAux) ? dataAux : default,
                                        GrauInstrucao = dr["Grau de Instrução"].ToString(),
                                        Curso = dr["Curso"].ToString()
                                    }).ToList();

                if (listaFuncionariosAtivos.Count() == 1)
                {
                    listaFuncionariosAtivos[0].TotalRegistros =  "Total de " + listaFuncionariosAtivos.Count().ToString() + " registro encontrado.";
                } else
                {
                    listaFuncionariosAtivos[0].TotalRegistros = "Total de " + listaFuncionariosAtivos.Count().ToString() + " registros encontrados.";
                }

                return listaFuncionariosAtivos;           
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarFuncionariosAtivos] " + ex.ToString());
            }
            finally
            {
                listaFuncionariosAtivos = null;
                sd.Connection.Close();
            }
        }

    }
}
