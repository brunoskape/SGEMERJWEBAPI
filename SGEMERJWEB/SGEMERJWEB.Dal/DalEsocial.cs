using SGEMERJWEB.Entidade;
using SGEMERJWEB.Entidade.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SGEMERJWEB.Dal
{
    public class DalEsocial : DalBase
    {
        private readonly string codUsuario;
        private readonly string idUsuario;

        public DalEsocial(string codusu, string idusu) : base(codusu)
        {
            codUsuario = string.IsNullOrEmpty(codusu) == true ? Environment.UserName : codusu ;
            idUsuario = string.IsNullOrEmpty(idusu) == true ? "0" : idusu;
        }

        /// <summary>
        /// Lista de eventos disponíveis para envio
        /// </summary>
        /// <param name="evento">Código do Evento. Ex: S-2300</param>
        /// <param name="nome">Nome ou parte do nome do colaborador</param>
        /// <param name="status">Tipos de Status possiveis: Todos = 0, Enviado = 1, Pendne = 2</param>
        /// <returns></returns>
        public IEnumerable<EsocialComunicador> RetornarEnvioEvento(string evento, string nome, string status, string cpf)
        {
            var lstEnvio = new List<EsocialComunicador>();

            try
            {
                var dtEnvio = sd.ExecutaProcDS("PKG_ESOCIAL.CNS_ENVIO_EVENTO", sd.CriaRefCursor(), evento, nome, status,cpf).Tables[0];

                lstEnvio = (from DataRow dr in dtEnvio.Rows
                            select new EsocialComunicador
                            {
                                Id = int.Parse(dr["id"].ToString()),
                                Id_Colaborador = int.Parse(dr["id_colaborador"].ToString()),
                                Id_Tipo_Colaborador = int.Parse(dr["id_Tipo_Colaborador"].ToString()),
                                Evento = dr["evento"].ToString(),
                                DataCriacao = dr["data_criacao"].ToString(),
                                DataEnvio = dr["data_envio"].ToString(),
                                Nome = dr["nome"].ToString(),
                                Cpf = dr["cpf"].ToString(),
                                Status = dr["status"].ToString(),
                                CodigoStatus = int.Parse(dr["codigoStatus"].ToString()),
                                Recibo = dr["recibo"].ToString()
                            }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarEnvioEvento] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }

            return lstEnvio;
        }

        public IEnumerable<EventoEsocial> RetornarEventoS2300(string evento , string referencia, string valor)
        {
            var lstEvEsocial = new List<EventoEsocial>();
            var dtEsocial = new DataSet();

            try
            {
                if (valor.Trim() != "") {

                        dtEsocial = sd.ExecutaProcDS("PKG_ESOCIAL.CNS_EVENTO_S2300", 
                                                     sd.CriaRefCursor("cTrabalhador"), 
                                                     sd.CriaRefCursor("cDependente"),
                                                     valor);
                        string v_idComunicador = "";

                        if (dtEsocial.Tables["cTrabalhador"].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtEsocial.Tables["cTrabalhador"].Rows)
                            {
                                v_idComunicador = dr["idComunicador"].ToString();

                                var evEsocial = new EventoEsocial();

                                evEsocial.tabela = "TSVInicio";
                                evEsocial.tipoArquivo = 2;
                                evEsocial.tipoInscricao = 1;
                                evEsocial.nroInscricao = dr["nroInscricao"].ToString();
                                evEsocial.anoMesReferencia = dr["anoMesReferencia"].ToString();
                                evEsocial.id = v_idComunicador;
                                evEsocial.identificador = getIdentificador(v_idComunicador);

                                evEsocial.atributos = new List<AtributoEvento>();

                                var v_cep = dr["brasil.cep"].ToString();
                                var v_codigoPostal = dr["exterior.codPostal"].ToString();
                                var cnpjInstEnsino = dr["instEnsino.cnpjInstEnsino"].ToString();

                            //Pegando as coluna do data table ctrabalhador
                            foreach (DataColumn dc in dr.Table.Columns)
                                {
                                    Boolean blnIncluirColuna = true;

                                    if (dc.ColumnName.ToUpper().Substring(0, 6) == "BRASIL"){
                                        blnIncluirColuna = v_cep == "" ? false : true;
                                    }else{
                                        if (dc.ColumnName.ToUpper().Substring(0, 8) == "EXTERIOR"){
                                            blnIncluirColuna = v_codigoPostal == "" ? false : true;
                                        }else if (dc.ColumnName.ToUpper() == "TRABIMIG.TMPRESID" && dr[dc].ToString() == "") {
                                            blnIncluirColuna = false;
                                        }
                                        else if (dc.ColumnName.ToUpper() == "TRABIMIG.CONDING" && dr[dc].ToString() == ""){
                                            blnIncluirColuna = false;
                                        }
                                        else {
                                            blnIncluirColuna = true;
                                        }
                                    }

                                    if (string.IsNullOrEmpty(cnpjInstEnsino) == false){
                                        switch (dc.ColumnName.ToString().ToUpper()){
                                            case "INSTENSINO.NMRAZAO":
                                            case "INSTENSINO.DSCLOGRAD":
                                            case "INSTENSINO.NRLOGRAD":
                                            case "INSTENSINO.BAIRRO":
                                            case "INSTENSINO.CEP":
                                            case "INSTENSINO.CODMUNIC":
                                            case "INSTENSINO.UF":
                                                blnIncluirColuna = false;
                                                break;
                                        }
                                    }

                                    if (dc.ColumnName.ToUpper() != "IDCOMUNICADOR" &&
                                            dc.ColumnName.ToUpper() != "ANOMESREFERENCIA" &&
                                            dc.ColumnName.ToUpper() != "NROINSCRICAO" &&
                                            blnIncluirColuna == true){

                                            var atributoEv = new AtributoEvento();
                                            if (dc.ColumnName.ToUpper() == "SUPERVISORESTAGI.CPFSUPERVISOR"){
                                                atributoEv.atributo = "supervisorEstagio.cpfSupervisor";
                                            }else{
                                                atributoEv.atributo = dc.ColumnName;
                                            }
                                             atributoEv.valor = dr[dc].ToString();
                                            evEsocial.atributos.Add(atributoEv);
                                        }
                                    }

                                    //Filtrando o dependente pelo id do comunicador esocial e Atribuindo os dependentes
                                    DataRow[]  linhasEncontradas = dtEsocial.Tables["cDependente"].Select("idComunicador =" + v_idComunicador);
                                    if (linhasEncontradas.Length > 0){
                                        var lstGenerica = new  List<ListaGenerica>();
                                        //Pegando as coluna do data table cDependente
                                        foreach (DataRow drr in linhasEncontradas)
                                        {
                                            var objListaGenerica = new ListaGenerica();
                                            objListaGenerica.tabela = "trabalhador.dependente";
                                            objListaGenerica.atributos = new List<AtributoEvento>();
                                            foreach (DataColumn dcc in drr.Table.Columns){
                                                if (dcc.ColumnName.ToUpper() != "IDCOMUNICADOR"){
                                                    var atributoEvv = new AtributoEvento();
                                                    atributoEvv.atributo = dcc.ColumnName;
                                                    atributoEvv.valor = drr[dcc].ToString();
                                                    objListaGenerica.atributos.Add(atributoEvv);
                                                }
                                            }
                                            lstGenerica.Add(objListaGenerica);
                                        }
                                        evEsocial.listas=lstGenerica;
                                    }

                                lstEvEsocial.Add(evEsocial);
                            }
                        }
                }
                return lstEvEsocial;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarEventoS2300] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }

        public IEnumerable<EventoEsocial2205> RetornarEventoS2205(string evento, string referencia, string valor)
        {
            var lstEvEsocial = new List<EventoEsocial2205>();
            var dtEsocial = new DataSet();

            try
            {
                if (valor.Trim() != "")
                {

                    dtEsocial = sd.ExecutaProcDS("PKG_ESOCIAL.cns_evento_s2205",
                                                 sd.CriaRefCursor("cTrabalhador"),
                                                 sd.CriaRefCursor("cDependente"),
                                                 valor);
                    string v_idComunicador = "";

                    if (dtEsocial.Tables["cTrabalhador"].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtEsocial.Tables["cTrabalhador"].Rows)
                        {
                            v_idComunicador = dr["idComunicador"].ToString();

                            var evEsocial = new EventoEsocial2205();

                            evEsocial.tabela = "FuncionarioAltCadastral";
                            evEsocial.tipoArquivo = 2;
                            evEsocial.tipoInscricao = 1;
                            evEsocial.nroInscricao = dr["nroInscricao"].ToString();
                            evEsocial.anoMesReferencia = dr["anoMesReferencia"].ToString();
                            evEsocial.id = v_idComunicador;
                            evEsocial.identificador = getIdentificador(v_idComunicador);

                            evEsocial.atributos = new List<AtributoEvento>();

                            var v_cep = dr["brasil.cep"].ToString();
                            var v_codigoPostal = dr["exterior.codPostal"].ToString();

                            //Pegando as coluna do data table ctrabalhador
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                Boolean blnIncluirColuna = true;

                                if (dc.ColumnName.ToUpper().Substring(0, 6) == "BRASIL")
                                {
                                    blnIncluirColuna = v_cep == "" ? false : true;
                                }
                                else
                                {
                                    if (dc.ColumnName.ToUpper().Substring(0, 8) == "EXTERIOR")
                                    {
                                        blnIncluirColuna = v_codigoPostal == "" ? false : true;
                                    }
                                    else if (dc.ColumnName.ToUpper() == "TRABIMIG.TMPRESID" && dr[dc].ToString() == "")
                                    {
                                        blnIncluirColuna = false;
                                    }
                                    else if (dc.ColumnName.ToUpper() == "TRABIMIG.CONDING" && dr[dc].ToString() == "")
                                    {
                                        blnIncluirColuna = false;
                                    }
                                    else
                                    {
                                        blnIncluirColuna = true;
                                    }
                                }

                                if (dc.ColumnName.ToUpper() != "IDCOMUNICADOR" &&
                                    dc.ColumnName.ToUpper() != "ANOMESREFERENCIA" &&
                                    dc.ColumnName.ToUpper() != "NROINSCRICAO" &&
                                    blnIncluirColuna == true)
                                {

                                    var atributoEv = new AtributoEvento();
                                    if (dc.ColumnName.ToUpper() == "SUPERVISORESTAGI.CPFSUPERVISOR")
                                    {
                                        atributoEv.atributo = "supervisorEstagio.cpfSupervisor";
                                    }
                                    else
                                    {
                                        atributoEv.atributo = dc.ColumnName;
                                    }
                                    atributoEv.valor = dr[dc].ToString();
                                    evEsocial.atributos.Add(atributoEv);
                                }
                            }

                            //Filtrando o dependente pelo id do comunicador esocial e Atribuindo os dependentes
                            DataRow[] linhasEncontradas = dtEsocial.Tables["cDependente"].Select("idComunicador =" + v_idComunicador);
                            if (linhasEncontradas.Length > 0)
                            {
                                var lstGenerica = new List<ListaGenerica>();
                                //Pegando as coluna do data table cDependente
                                foreach (DataRow drr in linhasEncontradas)
                                {
                                    var objListaGenerica = new ListaGenerica();
                                    objListaGenerica.tabela = "trabalhador.dependente";
                                    objListaGenerica.atributos = new List<AtributoEvento>();
                                    foreach (DataColumn dcc in drr.Table.Columns)
                                    {
                                        if (dcc.ColumnName.ToUpper() != "IDCOMUNICADOR")
                                        {
                                            var atributoEvv = new AtributoEvento();
                                            atributoEvv.atributo = dcc.ColumnName;
                                            atributoEvv.valor = drr[dcc].ToString();
                                            objListaGenerica.atributos.Add(atributoEvv);
                                        }
                                    }
                                    lstGenerica.Add(objListaGenerica);
                                }
                                evEsocial.listas = lstGenerica;
                            }

                            lstEvEsocial.Add(evEsocial);
                        }
                    }
                }
                return lstEvEsocial;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarEventoS2205] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }

        public IEnumerable<EventoEsocial2306> RetornarEventoS2306(string evento, string referencia, string valor)
        {
            var lstEvEsocial = new List<EventoEsocial2306>();
            var dtEsocial = new DataSet();

            try
            {
                if (valor.Trim() != "")
                {

                    dtEsocial = sd.ExecutaProcDS("PKG_ESOCIAL.cns_evento_s2306",
                                                 sd.CriaRefCursor("cTrabalhador"),
                                                 valor);
                    string v_idComunicador = "";

                    if (dtEsocial.Tables["cTrabalhador"].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtEsocial.Tables["cTrabalhador"].Rows)
                        {
                            v_idComunicador = dr["idComunicador"].ToString();

                            var evEsocial = new EventoEsocial2306();

                            evEsocial.tabela = "TSVAlteracaoContrato";
                            evEsocial.tipoArquivo = 2;
                            evEsocial.tipoInscricao = 1;
                            evEsocial.nroInscricao = dr["nroInscricao"].ToString();
                            evEsocial.anoMesReferencia = dr["anoMesReferencia"].ToString();
                            evEsocial.id = v_idComunicador;
                            evEsocial.identificador = getIdentificador(v_idComunicador);

                            evEsocial.atributos = new List<AtributoEvento>();

                            //se tiver cnpjInstituição da instiuição de ensino não mandar o nome da razão social, dscLograd, bairro, nrlograd, cep, codmunic, uf
                            var cnpjInstEnsino = dr["instEnsino.cnpjInstEnsino"].ToString();

                            //Pegando as coluna do data table ctrabalhador
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                bool blnIncluirColuna = true;

                                if(string.IsNullOrEmpty(cnpjInstEnsino) == false) 
                                {
                                    switch(dc.ColumnName.ToString().ToUpper()){
                                        case "INSTENSINO.NMRAZAO":
                                        case "INSTENSINO.DSCLOGRAD":
                                        case "INSTENSINO.NRLOGRAD":
                                        case "INSTENSINO.BAIRRO":
                                        case "INSTENSINO.CEP":
                                        case "INSTENSINO.CODMUNIC":
                                        case "INSTENSINO.UF":
                                            blnIncluirColuna = false;
                                            break;
                                    }
                                }

                                if (dc.ColumnName.ToUpper() != "IDCOMUNICADOR" &&
                                    dc.ColumnName.ToUpper() != "ANOMESREFERENCIA" &&
                                    dc.ColumnName.ToUpper() != "NROINSCRICAO" &&
                                    blnIncluirColuna == true)
                                {
                                    var atributoEv = new AtributoEvento();
                                    if (dc.ColumnName.ToUpper() == "SUPERVISORESTAGI.CPFSUPERVISOR"){
                                        atributoEv.atributo = "supervisorEstagio.cpfSupervisor";
                                    }else{
                                        atributoEv.atributo = dc.ColumnName;
                                    }
                                    atributoEv.valor = dr[dc].ToString();
                                    evEsocial.atributos.Add(atributoEv);
                                }
                            }                           
                            lstEvEsocial.Add(evEsocial);
                        }
                    }
                }
                return lstEvEsocial;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarEventoS2306] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }


        public IEnumerable<EventoEsocial2399> RetornarEventoS2399(string evento, string referencia, string valor)
        {
            var lstEvEsocial = new List<EventoEsocial2399>();
            var dtEsocial = new DataSet();

            try
            {
                if (valor.Trim() != "")
                {

                    dtEsocial = sd.ExecutaProcDS("PKG_ESOCIAL.cns_evento_s2399",
                                                 sd.CriaRefCursor("cRescisao"),
                                                 valor);
                    string v_idComunicador = "";

                    if (dtEsocial.Tables["cRescisao"].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtEsocial.Tables["cRescisao"].Rows)
                        {
                            v_idComunicador = dr["idComunicador"].ToString();

                            var evEsocial = new EventoEsocial2399();

                            evEsocial.tabela = "TSVTermino";
                            evEsocial.tipoArquivo = 2;
                            evEsocial.tipoInscricao = 1;
                            evEsocial.nroInscricao = dr["nroInscricao"].ToString();
                            evEsocial.anoMesReferencia = dr["anoMesReferencia"].ToString();
                            evEsocial.id = v_idComunicador;
                            evEsocial.identificador = getIdentificador(v_idComunicador);

                            evEsocial.atributos = new List<AtributoEvento>();

                            //Pegando as coluna do data table ctrabalhador
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                bool blnIncluirColuna = true;

                                if (dc.ColumnName.ToUpper() != "IDCOMUNICADOR" &&
                                    dc.ColumnName.ToUpper() != "ANOMESREFERENCIA" &&
                                    dc.ColumnName.ToUpper() != "NROINSCRICAO" &&
                                    blnIncluirColuna == true)
                                {
                                    var atributoEv = new AtributoEvento();
                                    atributoEv.atributo = dc.ColumnName;
                                    atributoEv.valor = dr[dc].ToString();
                                    evEsocial.atributos.Add(atributoEv);
                                }
                            }
                            lstEvEsocial.Add(evEsocial);
                        }
                    }
                }
                return lstEvEsocial;
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarEventoS2399] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }


        public void gravarEnvioJsonMPS(string jsonEnviado, string evento, string referencia)
        {
            try { 

                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }
                trans = sd.BeginTransaction();
                sd.ExecutaProc("PKG_ESOCIAL.gravarJsonEnviadoMPS", jsonEnviado,evento,referencia);
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[gravarEnvioJsonMPS] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }

        public List<RetornoMensagem> gravarRetorno(IEnumerable<EsocialRelatorio> lista)
        {
            List<RetornoMensagem> lstRetorno = new List<RetornoMensagem>();
            try
            {
                if (sd.Connection.State == ConnectionState.Closed){
                    sd.Connection.Open();
                }
                trans = sd.BeginTransaction();

                RetornoMensagem msg = new RetornoMensagem();
                msg.sucesso = true;

                foreach (EsocialRelatorio linha in lista)
                {
                    if(linha.statusInconsistenciaRepetido > 0)
                    {
                        msg.sucesso = false;
                        sd.ExecutaProc("PKG_ESOCIAL.gravarRetornoEnvio",
                                                    linha.id,
                                                    linha.anoMes,
                                                    "",
                                                    "",
                                                    linha.statusInconsistenciaMsg,
                                                    "N",
                                                    linha.identificador);

                    } else if (linha.inconsistenciasAtributo.Count > 0 )//aqui encontrou inconsitencia para um determinado colaborador
                    {
                        msg.sucesso = false;
                        foreach (inconcistenciaAtributo inconcistencia in linha.inconsistenciasAtributo)
                        {
                            sd.ExecutaProc("PKG_ESOCIAL.gravarRetornoEnvio", 
                                           linha.id, 
                                           linha.anoMes,
                                           inconcistencia.campo, 
                                           inconcistencia.valor, 
                                           inconcistencia.mensagem,
                                           "N",
                                           linha.identificador);
                        }
                    }else{
                        sd.ExecutaProc("PKG_ESOCIAL.gravarRetornoEnvio", 
                                       linha.id, 
                                       linha.anoMes, 
                                       "",
                                       "", 
                                       "Enviado com sucesso, sem inconsistência.",
                                       "S",
                                       linha.identificador);
                    }
                };

                if (msg.sucesso == false){
                    msg.mensagem = "Envio realizado, mas foram encontradas algumas inconsistências no envio.";  
                }else{
                    msg.mensagem = "Envio realizado com sucesso.";
                }
               
                lstRetorno.Add(msg);

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[gravarRetorno] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }

            return lstRetorno;
        }


        public List<comunicadorInconsistencia> comunicadorInconsistencia(string idComunicador)
        {
            List<comunicadorInconsistencia> lista = new List<comunicadorInconsistencia>();
            try { 
                var dt = sd.ExecutaProcDS("PKG_ESOCIAL.cns_comunicador_inconsistencia", 
                                               sd.CriaRefCursor(),
                                               idComunicador).Tables[0];
                lista = (from DataRow dr in dt.Rows
                        select new comunicadorInconsistencia
                        {
                            IdComunicador = dr["id_comunicador"].ToString(),
                            Referencia = dr["referencia"].ToString(),
                            Campo = dr["campo"].ToString(),
                            Valor = dr["valor"].ToString(),
                            Mensagem = dr["mensagem"].ToString(),
                            DataEnvio = dr["data_envio"].ToString(),
                            Aceito = dr["ind_aceito"].ToString()
                        }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarEnvioEvento] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
            return lista;
        }

        public Boolean eventoOriginalEnviadoS2300(int idcolaborador,int idTipoColaborador)
        {
            try
            {   
                var retorno = sd.ExecutaFunc("PKG_ESOCIAL.fn_eventoS2300OriginalEnviado", 1, idcolaborador, idTipoColaborador);
                return (retorno == "S" ? true : false);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[eventoOriginalEnviadoS2300] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }

        public void gravarRetificadoraS2300(int idcolaborador, int idTipoColaborador, string evento,string recibo)
        {
            try
            {

                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }
                trans = sd.BeginTransaction();
                sd.ExecutaProc("PKG_ESOCIAL.gravarRetificadoraS2300", idcolaborador, idTipoColaborador, evento, recibo);
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[gravarRetificadoraS2300] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }
    
        public void gravarEsocialComunicadorAlteracao(Colaborador c)
        {
            try
            {
                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }
                trans = sd.BeginTransaction();

                var ind_evento2306Aberto = sd.ExecutaFunc("pkg_esocial.fn_EventoEsocialAberta", 1, c.Id, c.TipoColaborador.Id.ToString(),"S-2306");
                if (ind_evento2306Aberto == "N")
                {
                    var alterouEvento2306 = sd.ExecutaFunc("pkg_esocial.fn_alteracaoEvento2306",
                                                     1,
                                                     c.IdDoTipo.ToString(),
                                                     c.CPF,
                                                     c.DadoTSVE.Cargo,
                                                     c.DadoTSVE.CBOCargo.Id.ToString(),
                                                     c.DadoEstagio.NaturezaEstagio.Id.ToString(),
                                                     c.DadoEstagio.NivelEstagio.Id.ToString(),
                                                     c.DadoEstagio.AreaAtuacao,
                                                     c.DadoEstagio.ApoliceSeguro,
                                                     c.DadoEstagio.DataPrevistaTermino,
                                                     c.DadoEstagio.CNPJInstEnsino,
                                                     c.DadoEstagio.RazaoSocialInstEnsino,
                                                     c.DadoEstagio.EnderecoInstEnsino.NomeLogradouro,
                                                     c.DadoEstagio.EnderecoInstEnsino.NumeroLogradouro,
                                                     c.DadoEstagio.EnderecoInstEnsino.Bairro,
                                                     c.DadoEstagio.EnderecoInstEnsino.CEP,
                                                     c.DadoEstagio.EnderecoInstEnsino.Municipio,
                                                     c.DadoEstagio.EnderecoInstEnsino.UF.Id,
                                                     c.DadoEstagio.CNPJAgenteInt,
                                                     c.DadoEstagio.CPFSupervisor);
                    if (alterouEvento2306.ToUpper() == "S")
                    {
                        sd.ExecutaProc("PKG_ESOCIAL.inc_EsocialComunicAlteracao", c.Id, c.TipoColaborador.Id, "S-2306");
                    }
                }

                var ind_evento2205Aberto = sd.ExecutaFunc("pkg_esocial.fn_EventoEsocialAberta", 1, c.Id, c.TipoColaborador.Id.ToString(), "S-2205");
                if (ind_evento2205Aberto == "N")
                {
                    //Verificar e gravar o evento S-2205
                    List<EmailPessoa> v_email = (from email in c.Emails
                                                 where email.TipoContato == Entidade.Enum.TipoContatoEnum.Principal
                                                 select new EmailPessoa
                                                 {
                                                     Id = email.Id,
                                                     TipoContato = email.TipoContato,
                                                     Email = email.Email
                                                 }).ToList();

                    List<TelefonePessoa> v_telefone = (from telefone in c.Telefones
                                                       where telefone.TipoContato == Entidade.Enum.TipoContatoEnum.Principal
                                                       select new TelefonePessoa
                                                       {
                                                           Id = telefone.Id,
                                                           TipoContato = telefone.TipoContato,
                                                           DDD = telefone.DDD,
                                                           Numero = telefone.Numero
                                                       }).ToList();

                    string[] v_ArrayDeficiencias = (from deficiencias in c.Deficiencia.ClassificacaoDeficiencias
                                                    orderby deficiencias.Id ascending
                                                    select deficiencias.Id.ToString()).ToArray();
                    string str_deficiencias = string.Join("", v_ArrayDeficiencias);

                    List<Endereco> objEndereco = (from e in c.Enderecos.Cast<Endereco>()
                                                  where e.Tipo == Entidade.Enum.TipoEnderecoEnum.Principal
                                                  select new Endereco
                                                  {
                                                      Id = e.Id,
                                                      CEP = e.CEP,
                                                      NomeLogradouro = e.NomeLogradouro,
                                                      NumeroLogradouro = e.NumeroLogradouro,
                                                      Bairro = e.Bairro,
                                                      Municipio = e.Municipio,
                                                      TipoLogradouro = e.TipoLogradouro,
                                                      Tipo = e.Tipo,
                                                      Complemento = e.Complemento,
                                                      UF = e.UF
                                                  }).ToList();

                    if (objEndereco.Count == 0)
                    {
                        objEndereco.Add(
                            new Endereco
                            {
                                Id = 0,
                                CEP = "",
                                NomeLogradouro = "",
                                NumeroLogradouro = "",
                                Bairro = "",
                                Municipio = "",
                                TipoLogradouro = new TipoLogradouro
                                {
                                    Id = 0
                                },

                                Tipo = 0,
                                Complemento = "",
                                UF = new UnidadeFederacao
                                {
                                    Id = ""
                                }
                            }
                            ); 
                    }
                        

                    var alterouEvento2205 = sd.ExecutaFunc("pkg_esocial.fn_alteracaoEvento2205",
                                                            1,
                                                            c.IdDoTipo.ToString(),
                                                            c.Nome.ToString(),
                                                            c.CPF,
                                                            c.PaisNascimento.Id.ToString(),
                                                            c.Sexo,
                                                            c.Raca.Id.ToString(),
                                                            c.EstadoCivil.Id.ToString(),
                                                            c.GrauInstrucao.Id.ToString(),
                                                            c.NomeSocial,
                                                            v_email[0].Email,
                                                            v_telefone[0].DDD.ToString() + v_telefone[0].Numero.ToString(),
                                                            c.TempoResidenciaEstrangeiro.Id.ToString(),
                                                            c.ClassificacaoTrabEstrangeiro.Id.ToString(),
                                                            c.ResidenciaExterior.PaisResidencia.Id.ToString(),
                                                            c.ResidenciaExterior.Descricao,
                                                            c.ResidenciaExterior.Numero,
                                                            c.ResidenciaExterior.Complemento,
                                                            c.ResidenciaExterior.Bairro,
                                                            c.ResidenciaExterior.Cidade,
                                                            c.ResidenciaExterior.CodigoPostal,
                                                            objEndereco[0].CEP,
                                                            objEndereco[0].TipoLogradouro.Id.ToString(),
                                                            objEndereco[0].NomeLogradouro,
                                                            objEndereco[0].NumeroLogradouro,
                                                            objEndereco[0].Complemento,
                                                            objEndereco[0].Bairro,
                                                            objEndereco[0].Municipio,
                                                            objEndereco[0].UF.Id,
                                                            c.Deficiencia.ObservacaoDeficiencia,
                                                            c.Deficiencia.EhReabilitadoReadaptado == true ? "S" : "N",
                                                            str_deficiencias);

                    //testar se houve mudança nos dependentes
                    string v_IndAlterouDependente = "N";
                    int? v_idDepAux;
                    foreach (Dependente dep in c.Dependentes)
                    {
                        v_idDepAux = dep.idDependenteAux;

                        if (dep.idDependenteAux == 0 || v_idDepAux == null)
                        {
                            v_IndAlterouDependente = "S";
                            break;
                        };

                        v_IndAlterouDependente = sd.ExecutaFunc("pkg_esocial.fn_AltDependenteEvento2205",
                                                                1,
                                                                c.IdDoTipo.ToString(),
                                                                dep.Id,
                                                                dep.TipoDependente.Id,
                                                                dep.Nascimento,
                                                                dep.CPF,
                                                                dep.Nome,
                                                                dep.TemIRRF == true ? "S" : "N",
                                                                dep.TemSalarioFamilia == true ? "S" : "N",
                                                                dep.TemIncapacidadeFisicaMental == true ? "S" : "N");
                        if (v_IndAlterouDependente == "S")
                        {
                            break;
                        }
                    }
                    if (alterouEvento2205.ToUpper() == "S" || v_IndAlterouDependente == "S")
                    {
                        sd.ExecutaProc("PKG_ESOCIAL.inc_EsocialComunicAlteracao", c.Id, c.TipoColaborador.Id, "S-2205");
                    }
                }

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[gravarEsocialComunicadorAlteracao] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }

        }

        private string getIdentificador(string idComunicador)
        {
            return DateTime.Now.ToString().Replace("/","").Replace(" ","").Replace(":","")+";E;5;"+idComunicador+";11;"+ idUsuario;
        }

        public string importarRelatorioDados(EsocialRelatorioDados dados)
        {
            try
            {
                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }
                trans = sd.BeginTransaction();
                if (dados.Registros.Count > 0)
                {
                    string referencia = dados.Referencia;
                    string tipoFuncionario = "1";
                    string evento = dados.Evento;
                    string status = "";
                    string idComunicador;
                    string reciboesocial;

                    foreach (Registro linha in dados.Registros)
                    {
                        if (linha.StatusDescricao.ToUpper()  == "Erro - Conteudo do evento inválido.".ToUpper())
                        {
                            status = "Recusado";
                        }else if (linha.StatusDescricao.ToUpper() == "Aceito - Sucesso.".ToUpper())
                        {
                            status = "Aceito";
                        }

                        idComunicador = linha.Identificador.Split(';')[3];
                        reciboesocial = linha.NrReciboPortal;

                        sd.ExecutaProc("PKG_ESOCIAL.gravarRetornoDadosEsocial",
                                        linha.Identificador,
                                        evento,
                                        referencia,
                                        tipoFuncionario,
                                        linha.StatusDescricao,
                                        status,
                                        idComunicador,
                                        reciboesocial); 
                    };
                }
                trans.Commit();
                return "IMPORTADO";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[importarRelatorioDados] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }


        public IEnumerable<EsocialDadosRelatorio> RelatorioDadosEsocialPaginado(string evento, 
                                                                                string referencia, 
                                                                                int? status = 0, 
                                                                                string cpf = "", 
                                                                                string nome ="")
        {
            var lst = new List<EsocialDadosRelatorio>();
            try
            {
                var dtEnvio = sd.ExecutaProcDS("PKG_ESOCIAL.cns_retornoDadosEsocial", 
                                                sd.CriaRefCursor(), 
                                                evento, 
                                                referencia, 
                                                status, 
                                                cpf, 
                                                nome).Tables[0];

                lst = (from DataRow dr in dtEnvio.Rows
                        select new EsocialDadosRelatorio
                        {   identificadorMPS = dr["identificador"].ToString(),
                            evento = dr["evento"].ToString(),
                            competencia = dr["referencia"].ToString(),
                            colaborador = new Colaborador {Id = 0, 
                                                           IdDoTipo = 0, 
                                                           CPF = dr["cpf"].ToString(),
                                                           Nome = dr["nome"].ToString(),
                                                           TipoColaborador = new Entidade.Dominio.TipoColaborador {Id = int.Parse(dr["idtipocolaborador"].ToString()),
                                                                                                                   Descricao = dr["descTipoColab"].ToString()}},
                            retornoMpsEsocial = dr["retornoesocial"].ToString(),
                            status = dr["status"].ToString(),
                            reciboEsocial = dr["reciboEsocial"].ToString()
                        }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("[RelatorioDadosEsocialPaginado] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
            return lst;
        }


        public void importaErrosEsocial(EsocialRelatorioDadosErros dados)
        {
            try
            {
                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }

                trans = sd.BeginTransaction();

                if (dados.Registros.Count > 0)
                {
                    string idComunicador;
                    string codigoErro;
                    string mensagem;

                    foreach (Registro linha in dados.Registros)
                    {

                        idComunicador = linha.Identificador.Split(';')[3];

                        foreach (Erros erros in linha.Erros)
                        {
                            codigoErro = erros.Codigo;
                            mensagem = erros.Mensagem;

                            sd.ExecutaProc("PKG_ESOCIAL.gravarRetornoErrosEsocial",
                                            linha.Identificador,
                                            codigoErro,
                                            mensagem);
                        }
                    };
                }

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[importaErrosEsocial] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }


        public List<ErrosEsocial> consultaRetornoErrosEsocial(string Identificador)
        {
            List<ErrosEsocial> lista = new List<ErrosEsocial>();
            try
            {
                var dt = sd.ExecutaProcDS("PKG_ESOCIAL.consultaRetornoErrosEsocial",
                                               sd.CriaRefCursor(),
                                               Identificador).Tables[0];
                lista = (from DataRow dr in dt.Rows
                         select new ErrosEsocial
                         {
                             Identificador = dr["Identificador"].ToString(),
                             CodigoErro = dr["CodigoErro"].ToString(),
                             Mensagem = dr["Mensagem"].ToString()
                         }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarEnvioEvento] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
            return lista;
        }

        public void gravarRetificadora(int idcolaborador, int idTipoColaborador, string evento, string recibo)
        {
            try
            {

                if (sd.Connection.State == ConnectionState.Closed)
                {
                    sd.Connection.Open();
                }
                trans = sd.BeginTransaction();
                sd.ExecutaProc("PKG_ESOCIAL.gravarRetificadoras", idcolaborador, idTipoColaborador, evento, recibo);
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception("[gravarRetificadoras] " + ex.ToString());
            }
            finally
            {
                sd.Connection.Close();
            }
        }


    }
}
