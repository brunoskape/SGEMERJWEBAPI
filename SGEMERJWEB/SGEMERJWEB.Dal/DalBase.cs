using EstruturaPadrao;
using Oracle.ManagedDataAccess.Client;
using ServicoDadosODPNETM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App = System.Configuration.ConfigurationManager;

namespace SGEMERJWEB.Dal
{
    public class DalBase
    {
        private readonly string strAmbiente;

        protected OracleTransaction trans;

        protected ServicoDadosOracle sd;
        protected OracleParameter cursor;

        private string UsuarioSO;

        public DalBase(string codusu)
        {
            strAmbiente = App.AppSettings["AmbienteApp"];

            UsuarioSO = codusu;

            sd = new ServicoDadosOracle
            {
                ConnectionString = ObterStringConexao,
                Usuario = GetEstrUsuario,
                SetarSchemaCorrente = ObterAmbienteSchema,
                OwnerGlobais = ObterAmbienteSchema
            };

            sd.Connection.TnsAdmin = App.AppSettings["TNSADMIN"].ToString() + string.Empty;

            cursor = sd.CriaRefCursor();
        }

        protected string ObterStringConexao
        {
            get
            {
                DataProtection.OracleConnectionStringProtector dp = new DataProtection.OracleConnectionStringProtector();

                string senhaConexao = App.AppSettings["SenhaConexao." + strAmbiente];
                string chaveCriptografada = App.AppSettings["ChaveCriptografada"];

                dp.SecureKey = chaveCriptografada;
                string senhaDecrypt = dp.Decrypt(senhaConexao);

                string strConexao = App.AppSettings["StringConexao." + strAmbiente].Replace("#SENHA#", senhaDecrypt);

                return strConexao;
            }
        }

        protected string ObterAmbienteSchema
        {
            get
            {
                string schema;

                switch (strAmbiente)
                {
                    case "D":
                        schema = "EMERJ_PUC";
                        break;

                    case "H":
                        schema = "EMERJ_PUC";
                        break;

                    case "P":
                        schema = "EMERJ_PUC";
                        break;

                    default:
                        throw new Exception("Não existe schema de ambiente definido.");
                }

                return schema;
            }
        }

        public EstruturaIdentificacaoUsuario GetEstrUsuario
        {
            get
            {
                EstruturaIdentificacaoUsuario usuario = new EstruturaIdentificacaoUsuario
                {
                    Login = "EMERJ_PUC",
                    NomeMaquina = Environment.MachineName,
                    UsuarioSO = string.IsNullOrEmpty(UsuarioSO) ? Environment.UserName : UsuarioSO,
                    CodOrg = 510,
                    LoginAutent = "",
                    SiglaSist = "SGEMERJWEB"
                };

                return usuario;
            }
        }

        protected string TrataBoolean(bool condicao)
        {
            return condicao ? "S" : "N";
        }

        protected bool TrataBoolean(string condicao)
        {
            return condicao == "S";
        }

        protected int? TratarZeroComoNulo(int valor)
        {
            if (valor > 0)
                return valor;

            return null;
        }

        protected int? TratarZeroComoNulo(int? valor)
        {
            if (valor > 0)
                return valor;

            return null;
        }

        protected long? TratarZeroComoNulo(long valor)
        {
            if (valor > 0)
                return valor;

            return null;
        }

        protected long? TratarZeroComoNulo(long? valor)
        {
            if (valor > 0)
                return valor;

            return null;
        }
    }
}
