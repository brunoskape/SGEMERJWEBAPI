using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SGEMERJWEB.Entidade;

namespace SGEMERJWEB.Dal
{
    public class DalCEP : DalBase
    {
        public DalCEP(string codusu) : base(codusu)
        {
        }

        public Endereco ObterEnderecoPeloCEP(string cep)
        {
            var endereco = new Endereco();

            try
            {
                if (string.IsNullOrEmpty(cep))
                    return null;


                var dtRetorno = sd.ExecutaProcDS("CEP_S_PESQUISA", sd.CriaRefCursor(), cep).Tables[0];

                endereco = (from DataRow dr in dtRetorno.Rows
                            select new Endereco
                            {
                                CEP = dr["cep"].ToString().Replace("-", ""),
                                NomeLogradouro = dr["log_no"].ToString(),
                                Bairro = dr["bai_no"].ToString(),
                                Municipio = dr["loc_no"].ToString(),
                                UF = { Id = dr["ufe_sg"].ToString() },
                                TipoLogradouro = { Descricao = dr["log_tipo_logradouro"].ToString() }
                            }).FirstOrDefault();

                return endereco;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarCEP] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }


    }
}
