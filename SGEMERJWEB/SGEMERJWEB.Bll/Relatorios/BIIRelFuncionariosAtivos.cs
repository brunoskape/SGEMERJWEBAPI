using SGEMERJWEB.Dal.Relatorios;
using SGEMERJWEB.Entidade.Relatorios;
using System;
using System.Collections.Generic;

namespace SGEMERJWEB.Bll.Relatorios
{
    public class BllRelFuncionariosAtivos
    {
        private readonly DalRelFuncionariosAtivos dalRelFuncionariosAtivos;

        public BllRelFuncionariosAtivos(string codusu)
        {
            dalRelFuncionariosAtivos = new DalRelFuncionariosAtivos(codusu);
        }

        public IEnumerable<FuncionariosAtivos> RetornarFuncionariosAtivos(int tipo, string dataInicio, string dataFim)
        {
            try
            {
                var listaFuncionariosAtivos = dalRelFuncionariosAtivos.RetornarFuncionariosAtivos(tipo, dataInicio, dataFim);

                return listaFuncionariosAtivos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


    }
}
