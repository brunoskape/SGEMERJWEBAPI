using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGEMERJWEB.WebAPI.Util
{
    public class Constante
    {
        #region Parametros
        readonly static public string P_STATUS_OK = "OK";
        readonly static public string P_STATUS_NOK = "NOK";
        readonly static public int P_TOTAL_REGISTROS_POR_PAGINA = 10;
        #endregion

        #region Mensagem
        readonly static public string M_REGISTRO_NAO_ENCONTRADO = "Não foram localizados registros para os filtros informados!";
        #endregion
    }
}