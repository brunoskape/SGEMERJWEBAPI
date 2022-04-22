using SGEMERJWEB.Dal;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SGEMERJWEB.Bll
{
    public class BllRescisao
    {
        private readonly DalRescisao objetoDal;

        public BllRescisao(string codusu)
        {
            objetoDal = new DalRescisao(codusu);
        }

        public BllRescisao(string codusu, string idusu)
        {
            objetoDal = new DalRescisao(codusu, idusu);
        }

        public Colaborador ObterRescisaoPorId(string value)
        {
            try
            {
                var colaborador = objetoDal.ObterRescisaoPorId(value);
                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public string Incluir(Colaborador objeto)
        {
            try
            {
                var resultado = objetoDal.incluir(objeto);
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString(), ex.InnerException);
            }
        }

        public string Alterar(Colaborador objeto)
        {
            try
            {
                var resultado = objetoDal.alterar(objeto);
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString(), ex.InnerException);
            }
        }

    }
}
