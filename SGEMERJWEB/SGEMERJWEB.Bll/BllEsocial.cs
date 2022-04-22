using SGEMERJWEB.Dal;
using SGEMERJWEB.Entidade;
using SGEMERJWEB.Entidade.Dominio;
using SGEMERJWEB.Entidade.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGEMERJWEB.Bll
{
    public class BllEsocial
    {
        private DalEsocial _dalEsocial;

        public BllEsocial(string codusu, string idusu)
        {
            _dalEsocial = new DalEsocial(codusu,idusu);
        }

        public (int, int, int, IEnumerable<EsocialComunicador>) RetornarEnvioPaginado(string e, string n, string s, int p, string cpf)
        {
            try
            {
                int pagina = p;
                const int maxLista = 10;

                if (pagina != 0)
                {
                    pagina -= 1;
                }

                var lstEnvioEv = _dalEsocial.RetornarEnvioEvento(e, n, s,cpf);
                var totRegistro = lstEnvioEv.Count();
                var totPagina = (int)Math.Ceiling((totRegistro / (decimal)maxLista));

                if (pagina > totPagina)
                {
                    pagina = 0;
                }

                var lstEnvioEvPg = lstEnvioEv.Skip(pagina * maxLista)
                                             .Take(maxLista).ToList();

                pagina++;

                return (pagina, totPagina, totRegistro, lstEnvioEvPg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public IEnumerable<EventoEsocial> RetornarEventoS2300(string evento = "S-2300", string referencia = "", string valor = "")
        {
            try
            {
                return _dalEsocial.RetornarEventoS2300(evento, referencia, valor);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public IEnumerable<EventoEsocial2306> RetornarEventoS2306(string evento,  string referencia, string valor)
        {
            try
            {
                return _dalEsocial.RetornarEventoS2306(evento, referencia, valor);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public IEnumerable<EventoEsocial2205> RetornarEventoS2205(string evento, string referencia, string valor)
        {
            try
            {
                return _dalEsocial.RetornarEventoS2205(evento, referencia, valor);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public IEnumerable<EventoEsocial2399> RetornarEventoS2399(string evento, string referencia, string valor)
        {
            try
            {
                return _dalEsocial.RetornarEventoS2399(evento, referencia, valor);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public void gravarEnvioJsonMPS(string jsonEnviado,string evento,string referencia)
        {
            _dalEsocial.gravarEnvioJsonMPS(jsonEnviado,evento,referencia);
        }

        public List<RetornoMensagem> gravarRetorno(IEnumerable<EsocialRelatorio> lista)
        {
            List<RetornoMensagem> lstRetorno = null;
            lstRetorno = _dalEsocial.gravarRetorno(lista);
            return lstRetorno;
        }

        public List<comunicadorInconsistencia> comunicadorInconsistencia(string idComunicador)
        {
            try
            {
                return _dalEsocial.comunicadorInconsistencia(idComunicador);
            }
            catch (Exception ex)
            {
                throw new Exception("[RetornarEnvioEvento] " + ex.ToString());
            }
        }

        public Boolean eventoOriginalEnviadoS2300(int idcolaborador, int idTipoColaborador)
        {
            try
            {
                return _dalEsocial.eventoOriginalEnviadoS2300 (idcolaborador, idTipoColaborador);
            }
            catch (Exception ex)
            {
                throw new Exception("[eventoOriginalEnviadoS2300] " + ex.ToString());
            }
        }

        public string importarRelatorioDados(EsocialRelatorioDados dados)
        {
            try
            {
                 return _dalEsocial.importarRelatorioDados(dados);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public (int, int, int, IEnumerable<EsocialDadosRelatorio>) RelatorioDadosEsocialPaginado(string evento, 
                                                                                                 string referencia, 
                                                                                                 int? status,
                                                                                                 string cpf,
                                                                                                 string nome,
                                                                                                 int p)
        {
            try
            {
                int pagina = p;
                const int maxLista = 10;

                if (pagina != 0)
                {
                    pagina -= 1;
                }

                var lstEnvioEv = _dalEsocial.RelatorioDadosEsocialPaginado(evento, referencia, status, cpf, nome);
                var totRegistro = lstEnvioEv.Count();
                var totPagina = (int)Math.Ceiling((totRegistro / (decimal)maxLista));

                if (pagina > totPagina)
                {
                    pagina = 0;
                }
                var lstEnvioEvPg = lstEnvioEv.Skip(pagina * maxLista)
                                             .Take(maxLista).ToList();
                pagina++;
                return (pagina, totPagina, totRegistro, lstEnvioEvPg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public void importaErrosEsocial(EsocialRelatorioDadosErros dados)
        {
            try
            {
                _dalEsocial.importaErrosEsocial(dados);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        public List<ErrosEsocial> consultaRetornoErrosEsocial(string identificador)
        {
            try
            {
                return _dalEsocial.consultaRetornoErrosEsocial(identificador);
            }
            catch (Exception ex)
            {
                throw new Exception("[consultaRetornoErrosEsocial] " + ex.ToString());
            }
        }


    }
}
