using DGTECGEDARDOTNET;
using System;
using System.IO;
using System.Web;
using App = System.Configuration.ConfigurationManager;

namespace GedAr
{
    public class Ged
    {
        private readonly DGTECGEDAR objged;
        private readonly string ambienteApp = App.AppSettings["AmbienteApp"];
        public string path;
        public Ged()
        {
            objged = new DGTECGEDAR();

            path = HttpContext.Current.Server.MapPath("~/temp/DocumentosGED");
            //Cria o diretório caso não exista na pasta temp
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Inicializa o objeto GED
        /// </summary>
        private void ConectaGed()
        {
            try
            {
                if (ambienteApp == "P")
                {
                    objged.Inicializa("SGEMERJ", "1", Environment.MachineName, "SGEMERJ", Environment.UserName, "1", DGTECGEDAR.TipoServidorIndexacao.Producao1);
                }
                else
                {
                    objged.Inicializa("SGEMERJ", "1", Environment.MachineName, "SGEMERJ", Environment.UserName, "1", DGTECGEDAR.TipoServidorIndexacao.Homologacao1);
                }
            }
            catch (DGTECGEDARException gedEx)
            {
                objged.Finaliza();
                throw new Exception(gedEx.ToString());
            }
            catch (Exception ex)
            {
                objged.Finaliza();
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// Retorna URL completa para download pelo navegador
        /// </summary>
        /// <param name="idGed"></param>
        /// <returns></returns>
        public string RetornarUrlGed(string idGed)
        {
            var url = string.Empty;
            bool cacheDisp = false;

            try
            {
                ConectaGed();

                url = objged.MontaURLCacheWeb(idGed, 0, "", "", ref cacheDisp, false, false, true);
            }
            catch (DGTECGEDARException gedEx)
            {
                throw new Exception(gedEx.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                objged.Finaliza();
            }

            return url;
        }

        /// <summary>
        /// Retorna a Identificação do documento armazenado no GED
        /// </summary>
        /// <param name="fileBase64">Arquivo (Array de bit) convertido para base64</param>
        /// <param name="nomeArquivo">Nome do arquivo que será salvo</param>
        /// <param name="path">Caminho onde será descarregado o arquivo para posterior armazenamento</param>
        /// <returns></returns>
        public string RetornarIdGed(string fileBase64, string nomeArquivo)
        {
            var idged = string.Empty;

            var content = Convert.FromBase64String(fileBase64);

            string pathFileSave = Path.Combine(path, nomeArquivo);

            try
            {
                ConectaGed();

                //Cria o diretório caso não exista na pasta temp
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //salvando o arquivo na pasta temporaria.
                File.WriteAllBytes(pathFileSave, content);

                //armazenamento no ged.
                idged = objged.Armazena(pathFileSave);

                //Exclui o arquivo salvo na pasta temporaria
                if (File.Exists(pathFileSave))
                {
                    File.Delete(pathFileSave);
                }
            }
            catch (DGTECGEDARException gedEx)
            {
                throw new Exception(gedEx.ToString());
            }
            catch (IOException ioEx)
            {
                throw new Exception(ioEx.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                objged.Finaliza();
            }

            return idged;
        }

        /// <summary>
        /// Substitui um documento no GED mantendo o mesmo Identificador
        /// </summary>
        /// <param name="idGed">Identificador do GED</param>
        /// <param name="fileBase64">Arquivo (Array de bit) convertido para base64</param>
        /// <param name="nomeArquivo">Nome do arquivo que será salvo</param>        
        /// <returns>Número da atual versão do documento armazenado</returns>
        public int Substituir(string idGed, string fileBase64, string nomeArquivo)
        {
            int alt;

            var content = Convert.FromBase64String(fileBase64);
            string pathFileSave = Path.Combine(path, nomeArquivo);

            try
            {
                ConectaGed();

                //Cria o diretório caso não exista na pasta temp
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //salvando o arquivo na pasta temporaria.
                File.WriteAllBytes(pathFileSave, content);

                //substituir no ged.
                alt = objged.Substitui(idGed, pathFileSave, false);

                //Exclui o arquivo salvo na pasta temporaria
                if (File.Exists(pathFileSave))
                {
                    File.Delete(pathFileSave);
                }
            }
            catch (DGTECGEDARException gedEx)
            {
                throw new Exception(gedEx.ToString());
            }
            catch (IOException ioEx)
            {
                throw new Exception(ioEx.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                objged.Finaliza();
            }

            return alt;
        }

        /// <summary>
        /// Exclui fisicamente um documento armazenado no GED
        /// </summary>
        /// <param name="idGed"></param>
        public int Excluir(string idGed)
        {
            try
            {
                ConectaGed();

                return objged.Exclui(idGed, false);
            }
            catch (DGTECGEDARException gedEx)
            {
                throw new Exception(gedEx.ToString());
            }
            catch (IOException ioEx)
            {
                throw new Exception(ioEx.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                objged.Finaliza();
            }
        }
    }
}
