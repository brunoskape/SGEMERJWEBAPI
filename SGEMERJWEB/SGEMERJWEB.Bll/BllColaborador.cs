using GedAr;
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
    public class BllColaborador
    {
        private readonly DalColaborador objetoDal;
        private readonly Ged GEDService;

        public BllColaborador(string codusu)
        {
            objetoDal = new DalColaborador(codusu);
            GEDService = new Ged();
        }

        public BllColaborador(string codusu, string idusu)
        {
            objetoDal = new DalColaborador(codusu,idusu);
            GEDService = new Ged();
        }


        public string Gravar(Colaborador colaborador)
        {
            try
            {
                if (colaborador != null)
                {
                    string erro = ValidarDados(colaborador);
                    if (string.IsNullOrEmpty(erro))
                    {
                        var resultado = objetoDal.Gravar(colaborador);
                        if (resultado.Equals("ALTERADO") || resultado.Equals("INCLUIDO") || resultado.Equals("INCLUIDO-TIPO"))
                        {
                            if (colaborador.FotoBase64 != null)
                            {
                                resultado = GravarDadosNoGED(colaborador, resultado);
                            }
                            else if (!string.IsNullOrEmpty(colaborador.IDGed))
                            {
                                resultado = RemoverDadosNoGED(colaborador, resultado);
                            }
                        }

                        return resultado;
                    }
                    else
                        return erro;
                }
                else
                {
                    return "Dados do objeto não informado!";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString(), ex.InnerException);
            }
        }

        public (int totalRegistros, int totalPaginas, IEnumerable<ColaboradorGridDTO> lista) ObterListaPaginada(string nome, string cpf, int tipo, int pagina, int regPagina)
        {
            try
            {
                var (totalRegistros, lista) = objetoDal.ObterListaPaginada(nome, cpf, tipo, pagina, regPagina);

                var totalPaginas = 0;
                if (lista.Any())
                {
                    totalPaginas = (int)Math.Ceiling((totalRegistros / (decimal)regPagina));
                }

                return (totalRegistros, totalPaginas, lista);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        public Colaborador Obter(int idDoTipo, bool comFoto = false)
        {
            try
            {
                var colaborador = objetoDal.Obter(idDoTipo);

                if (colaborador != null)
                {
                    if (!string.IsNullOrEmpty(colaborador.IDGed) && comFoto)
                    {
                        colaborador.FotoBase64 = ObterDadosDoGed(colaborador.IDGed);
                    }
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public Colaborador ObterPorTipoECPF(int tipo, string cpf, bool comFoto = false)
        {
            try
            {
                var colaborador = objetoDal.ObterPorTipoECPF(tipo, cpf);

                if (colaborador != null)
                {
                    if (!string.IsNullOrEmpty(colaborador.IDGed) && comFoto)
                    {
                        colaborador.FotoBase64 = ObterDadosDoGed(colaborador.IDGed);
                    }
                }

                return colaborador;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        #region GED Foto
        private string ObterDadosDoGed(string idGed)
        {
            string urlDoArquivo = "";
            try
            {
                string resultado = "";

                 urlDoArquivo = GEDService.RetornarUrlGed(idGed);

                if (!string.IsNullOrEmpty(urlDoArquivo))
                {
                    Guid guid = Guid.NewGuid();
                    var arquivoBaixado = Path.Combine(GEDService.path, guid.ToString());

                    var client = new WebClient();
                    client.DownloadFile(urlDoArquivo, arquivoBaixado);

                    //converter para bitmap para garantir que é uma imagem válida
                    //Bitmap.FromFile(arquivoBaixado);                    

                    resultado = "data:image/png;base64," + Convert.ToBase64String(File.ReadAllBytes(arquivoBaixado));

                    if (File.Exists(arquivoBaixado))
                    {
                        File.Delete(arquivoBaixado);
                    }
                }
                return resultado;
            }
            catch
            {
                return "";
            }
        }

        private string GravarDadosNoGED(Colaborador colaborador, string resultadoInicial)
        {
            if (colaborador.FotoBase64.Contains(","))
                colaborador.FotoBase64 = colaborador.FotoBase64.Split(',')[1];

            string nomeArquivo = "foto_" + colaborador.CPF.ToString();

            if (string.IsNullOrEmpty(colaborador.IDGed))
            {

                var idgerado = GEDService.RetornarIdGed(colaborador.FotoBase64, nomeArquivo);
                if (!string.IsNullOrEmpty(idgerado))
                {
                    colaborador.IDGed = idgerado;

                    objetoDal.GravarIDDoGed(colaborador);

                    return resultadoInicial;
                }
                else
                {
                    return "Erro ao gravar dados no GED";
                }

            }
            else
            {
                _ = GEDService.Substituir(colaborador.IDGed, colaborador.FotoBase64, nomeArquivo);
                return resultadoInicial;

            }

        }


        private string RemoverDadosNoGED(Colaborador colaborador, string resultadoInicial)
        {
            _ = GEDService.Excluir(colaborador.IDGed);

            colaborador.IDGed = null;
            objetoDal.GravarIDDoGed(colaborador);

            return resultadoInicial;

        }
        #endregion

        #region Validações

        private string ValidarDados(Colaborador colaborador)
        {
            string erro = ValidarDadosBasicos(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarMudancaCPF(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            bool cpfExiste = objetoDal.CPFJaExiste(colaborador.Id, colaborador.CPF);
            if (cpfExiste)
                return "Já existe um colaborador cadastrado com esse CPF.";

            erro = ValidarEndereco(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarDeficiencia(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarRemuneracao(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarResidenteExterior(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarTrabalhadorImigrante(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarDependente(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarInformacaoTSVE(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarInformacaoEstagio(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarInstituicaoDeEnsino(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarAfastamento(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarDadosProfissionais(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarCursoExtraCurricular(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;

            erro = ValidarInformacoesAcademica(colaborador);
            if (!string.IsNullOrEmpty(erro))
                return erro;


            return null;
        }
        private string ValidarDadosBasicos(Colaborador colaborador)
        {
            try
            {
                if (string.IsNullOrEmpty(colaborador.Nome))
                    return "Nome do colaborador é obrigatório.";

                if (string.IsNullOrEmpty(colaborador.CPF))
                    return "CPF do colaborador é obrigatório.";

                if (!ServiceUtil.EhCPFValido(colaborador.CPF))
                    return "CPF do colaborador inválido.";

                if (colaborador.Sexo != "M" && colaborador.Sexo != "F")
                    return "Sexo colaborador inválido.";


                if (colaborador.NascimentoConversao() == DateTime.MinValue || colaborador.NascimentoConversao() == DateTime.MaxValue)
                    return "Data de Nascimento do colaborador inválida.";

                if (colaborador.NascimentoConversao() > DateTime.Now)
                    return "Data de Nascimento do colaborador maior que a data atual.";

                var raca = BllDominioFachada.BllRaca().Obter(colaborador.Raca.Id);
                if (raca == null)
                    return "Raça do colaborador é inválida.";

                var estadoCivil = BllDominioFachada.BllEstadoCivil().Obter(colaborador.EstadoCivil.Id);
                if (estadoCivil == null)
                    return "Estado civíl do colaborador é inválido.";

                var grauInstrucao = BllDominioFachada.BllGrauInstrucao().Obter(colaborador.GrauInstrucao.Id);
                if (grauInstrucao == null)
                    return "Grau de instrução do colaborador é inválido.";

                var tipo = BllDominioFachada.BllTipoColaborador().Obter(colaborador.TipoColaborador.Id);
                if (tipo == null)
                    return "Tipo do colaborador é inválido.";

                var paisNascimento = BllDominioFachada.BllPais().Obter(colaborador.PaisNascimento.Id);
                if (paisNascimento == null)
                    return "País de nascimento do colaborador é inválido.";

                var paisNacionalidade = BllDominioFachada.BllPais().Obter(colaborador.PaisNacionalidade.Id);
                if (paisNacionalidade == null)
                    return "País de nacionalidade do colaborador é inválido.";


                if (!colaborador.EHorarioDinamicoTrabalho && colaborador.TipoColaborador.Id != 1)
                {
                    if (string.IsNullOrEmpty(colaborador.HorarioInicioTrabalho))
                        return "Horário de Início do Trabalho do colaborador é obrigatório.";

                    if (string.IsNullOrEmpty(colaborador.HorarioTerminoTrabalho))
                        return "Horário de Término do Trabalho  do colaborador é obrigatório.";

                    if (colaborador.HorarioInicioTrabalhoConversao() == null)
                        return "Horário de Início do Trabalho  do colaborador é inválido.";

                    if (colaborador.HorarioTerminoTrabalhoConversao() == null)
                        return "Horário de Término do Trabalho  do colaborador é inválido.";

                    if (colaborador.HorarioInicioTrabalhoConversao() > colaborador.HorarioTerminoTrabalhoConversao())
                        return "Horário de Início do Trabalho  do colaborador maior que o horário de fim.";
                }


                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarMudancaCPF(Colaborador colaborador)
        {
            try
            {
                if (!string.IsNullOrEmpty(colaborador.CPFNovo))
                {
                    if (!ServiceUtil.EhCPFValido(colaborador.CPFNovo))
                        return "CPF novo do colaborador inválido.";

                    if (!ServiceUtil.EhCPFValido(colaborador.CPFAntigo))
                        return "CPF antigo do colaborador inválido.";

                    if (colaborador.DataAlteracaoCPFConversao() == null)
                        return "Data de alteração do CPF do colaborador inválida.";

                    if (colaborador.DataAlteracaoCPFConversao() == DateTime.MinValue || colaborador.DataAlteracaoCPFConversao() == DateTime.MaxValue)
                        return "Data de alteração do CPF do colaborador inválida.";

                    if (colaborador.DataAlteracaoCPFConversao() > DateTime.Now)
                        return "Data de alteração do CPF do colaborador maior que a data atual.";

                    if (colaborador.CPFAntigo == colaborador.CPFNovo)
                        return "CPF novo do colaborador igual ao CPF antigo.";

                    //se o novo foi informado, assume como principal
                    colaborador.CPF = colaborador.CPFNovo;
                }
                else
                {
                    colaborador = objetoDal.PreencherTrocaDeCPFValores(colaborador);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarEndereco(Colaborador colaborador)
        {
            try
            {
                colaborador.Enderecos.RemoveAll(s => string.IsNullOrEmpty(s.CEP));

                foreach (var endereco in colaborador.Enderecos)
                {

                    if (endereco.TipoLogradouro == null)
                        return "Tipo de colaborador inválido.";

                    if (endereco.TipoLogradouro.Id <= 0)
                        return "Tipo de colaborador inválido.";

                    var tipoDeLogradouro = BllDominioFachada.BllTipoLogradouro().Obter(endereco.TipoLogradouro.Id);
                    if (tipoDeLogradouro == null)
                        return "Tipo de Logradouro do colaborador é inválido.";

                    if (string.IsNullOrEmpty(endereco.NumeroLogradouro))
                        endereco.NumeroLogradouro = "S/N";

                    if (string.IsNullOrEmpty(endereco.NomeLogradouro))
                        return "Logradouro do colaborador é inválido.";

                    if (string.IsNullOrEmpty(endereco.Bairro))
                        return "Bairro do colaborador é inválido.";

                    if (string.IsNullOrEmpty(endereco.Municipio))
                        return "Município do colaborador é inválido.";

                    if (endereco.UF == null)
                        return "UF do colaborador é inválida.";

                    if (string.IsNullOrEmpty(endereco.UF.Id))
                        return "UF do colaborador é inválida.";

                    var ufDoLogradouro = new BllUnidadeFederacao().Obter(endereco.UF.Id);
                    if (ufDoLogradouro == null)
                        return "UF do colaborador é inválida.";

                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarDeficiencia(Colaborador colaborador)
        {
            try
            {
                if (colaborador.Deficiencia.EhPortadorDeNecessidadeEspecial)
                {
                    if (!colaborador.Deficiencia.ClassificacaoDeficiencias.Any())
                        return "Deficiência(s) do colaborador não informada.";

                    foreach (var classificacao in colaborador.Deficiencia.ClassificacaoDeficiencias)
                    {
                        var deficiencia = BllDominioFachada.BllClassificacaoDeficiencia().Obter(classificacao.Id);
                        if (deficiencia == null)
                            return "Deficiência inválida. [" + classificacao.Id.ToString() + "]";
                    }
                }
                else
                {
                    colaborador.Deficiencia.ClassificacaoDeficiencias = new List<Entidade.Dominio.ClassificacaoDeficiencia>();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarRemuneracao(Colaborador colaborador)
        {
            try
            {
                if (colaborador.TipoColaborador.Id == 2 || colaborador.TipoColaborador.Id == 3) //dados obrigatórios para prestador ou servidor
                {
                    if (colaborador.UnidadePagamento == null)
                        return "Unidade de Pagamento do colaborador não informada.";

                    if (colaborador.UnidadePagamento.Id == 0)
                        return "Unidade de Pagamento do colaborador não informada.";

                    if (colaborador.UnidadePagamento.Id == 7)
                        colaborador.SalarioBase = 0;
                    else
                    {
                        if (colaborador.SalarioBase == null)
                            return "Salário Base do colaborador não informado.";
                        if (colaborador.SalarioBase <= 0)
                            return "Salário Base do colaborador não informado.";
                    }

                }
                else
                {
                    colaborador.UnidadePagamento = new UnidadePagamento();
                    colaborador.SalarioBase = null;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        private string ValidarTrabalhadorImigrante(Colaborador colaborador)
        {
            try
            {
                if (colaborador.PaisNascimento.Id != 1) // Brasil
                {
                    if (colaborador.TempoResidenciaEstrangeiro.Id <= 0)
                    {
                        return "Tempo de Residência do colaborador não informado.";
                    }

                    var TempoResidenciaEstrangeiro = BllDominioFachada.BllTempoResidenciaEstrangeiro().Obter(colaborador.TempoResidenciaEstrangeiro.Id);
                    if (TempoResidenciaEstrangeiro == null)
                        return "Tempo de Residência do colaborador inválida.";


                    if (colaborador.ClassificacaoTrabEstrangeiro.Id <= 0)
                    {
                        return "Classificação do Trab. Estrangeiro do colaborador não informada.";
                    }

                    var ClassificacaoTrabEstrangeiro = BllDominioFachada.BllClassificacaoTrabEstrangeiro().Obter(colaborador.ClassificacaoTrabEstrangeiro.Id);
                    if (ClassificacaoTrabEstrangeiro == null)
                        return "Classificação do Trab. Estrangeiro do colaborador inválida.";

                    if (colaborador.TempoResidenciaEstrangeiro.Id == 1) //prazo indeterminado
                    {
                        //não pode classificação igual a 2 e 5
                        if (colaborador.ClassificacaoTrabEstrangeiro.Id == 2 || colaborador.ClassificacaoTrabEstrangeiro.Id == 5)
                        {
                            return "Classificação do Trab. Estrangeiro do colaborador é inválida para o tempo de residência informado.";
                        }
                    }
                    else if (colaborador.TempoResidenciaEstrangeiro.Id == 2) //prazo determinado
                    {
                        //não pode classificação igual a 1
                        if (colaborador.ClassificacaoTrabEstrangeiro.Id == 1)
                        {
                            return "Classificação do Trab. Estrangeiro do colaborador é inválida para o tempo de residência informado.";
                        }
                    }
                }
                else
                {
                    colaborador.ClassificacaoTrabEstrangeiro = new ClassificacaoTrabEstrangeiro();
                    colaborador.TempoResidenciaEstrangeiro = new TempoResidenciaEstrangeiro();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarResidenteExterior(Colaborador colaborador)
        {
            try
            {
                if (!colaborador.Enderecos.Any())
                {
                    if (colaborador.ResidenciaExterior.PaisResidencia == null)
                        return "País de Residência no exterior do colaborador não informado.";

                    if (colaborador.ResidenciaExterior.PaisResidencia.Id <= 0)
                        return "País de Residência no exterior do colaborador não informado.";

                    var pais = BllDominioFachada.BllPais().Obter(colaborador.ResidenciaExterior.PaisResidencia.Id);
                    if (pais == null)
                        return "País de Residência no exterior do colaborador inválido.";

                    if (colaborador.ResidenciaExterior.PaisResidencia.Id == 1)
                        return "País de Residência no exterior do colaborador deve ser diferente de Brasil.";

                    if (string.IsNullOrEmpty(colaborador.ResidenciaExterior.CodigoPostal))
                        return "Código Postal da Residência no exterior do colaborador inválido.";

                    if (string.IsNullOrEmpty(colaborador.ResidenciaExterior.Descricao))
                        return "Descrição da Residência no exterior do colaborador inválido.";

                    if (string.IsNullOrEmpty(colaborador.ResidenciaExterior.Bairro))
                        return "Bairro da Residência no exterior do colaborador inválido.";

                    if (string.IsNullOrEmpty(colaborador.ResidenciaExterior.Cidade))
                        return "Nome da Cidade da Residência no exterior do colaborador inválido.";

                    if (string.IsNullOrEmpty(colaborador.ResidenciaExterior.Numero))
                        colaborador.ResidenciaExterior.Numero = "S/N";
                }


                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarDependente(Colaborador colaborador)
        {
            try
            {
                if (colaborador.Dependentes.Any())
                {
                    int i = 0;
                    foreach (var dependente in colaborador.Dependentes)
                    {
                        i++;

                        if (dependente.TipoDependente.Id <= 0)
                            return "Tipo de Dependente do colaborador inválido. [" + i.ToString() + "]";

                        var tipo = BllDominioFachada.BllTipoDependente().Obter(dependente.TipoDependente.Id);
                        if (tipo == null)
                            return "Tipo de Dependente do colaborador inválido. [" + i.ToString() + "]";

                        if (string.IsNullOrEmpty(dependente.Nome)) // Brasil
                            return "Nome do Dependente do colaborador inválido. [" + i.ToString() + "]";

                        if (dependente.NascimentoConversao() == DateTime.MinValue || dependente.NascimentoConversao() == DateTime.MaxValue)
                            return "Data de Nascimento do Dependente do colaborador inválida. [" + i.ToString() + "]";

                        if (dependente.NascimentoConversao() > DateTime.Now)
                            return "Data de Nascimento do Dependente do colaborador maior que a data atual. [" + i.ToString() + "]";

                        if (dependente.TemIRRF)
                        {
                            if (string.IsNullOrEmpty(dependente.CPF))
                                return "CPF do Dependente do colaborador é obrigatório. [" + i.ToString() + "]";

                            if (!ServiceUtil.EhCPFValido(dependente.CPF))
                                return "CPF do Dependente do colaborador inválido. [" + i.ToString() + "]";
                        }
                        else if (!string.IsNullOrEmpty(dependente.CPF))
                        {
                            if (!ServiceUtil.EhCPFValido(dependente.CPF))
                                return "CPF do Dependente do colaborador inválido. [" + i.ToString() + "]";
                        }
                    }
                }


                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarInformacaoEstagio(Colaborador colaborador)
        {
            try
            {
                if (colaborador.TipoColaborador.Id == 1)
                {
                    if (colaborador.DadoEstagio.NaturezaEstagio == null)
                        return "Natureza de Estágio do colaborador inválida.";

                    if (colaborador.DadoEstagio.NaturezaEstagio.Id <= 0)
                        return "Natureza de Estágio do colaborador inválida.";

                    var natureza = BllDominioFachada.BllNaturezaEstagio().Obter(colaborador.DadoEstagio.NaturezaEstagio.Id);
                    if (natureza == null)
                        return "Natureza de Estágio do colaborador inválida.";

                    if (colaborador.DadoEstagio.NivelEstagio == null)
                        return "Nível de Estágio do colaborador inválida.";

                    if (colaborador.DadoEstagio.NivelEstagio.Id <= 0)
                        return "Nível de Estágio do colaborador inválido.";

                    var nivel = BllDominioFachada.BllNivelEstagio().Obter(colaborador.DadoEstagio.NivelEstagio.Id);
                    if (nivel == null)
                        return "Nível de Estágio do colaborador inválido.";

                    if (string.IsNullOrEmpty(colaborador.DadoEstagio.AreaAtuacao))
                        return "Área de Atuação do Estágio do colaborador é obrigatório.";

                    if (colaborador.DadoEstagio.DataInicioConversao() == DateTime.MinValue || colaborador.DadoEstagio.DataInicioConversao() == DateTime.MaxValue)
                        return "Data Início de Estágio do colaborador inválida.";

                    if (colaborador.DadoEstagio.DataPrevistaTerminoConversao() == DateTime.MinValue || colaborador.DadoEstagio.DataPrevistaTerminoConversao() == DateTime.MaxValue)
                        return "Data Prevista de Término de Estágio do colaborador inválida.";

                    if (colaborador.DadoEstagio.DataInicioConversao() >= colaborador.DadoEstagio.DataPrevistaTerminoConversao())
                        return "Data Início maior ou igual a Data Prevista de Término de Estágio do colaborador.";

                    if (colaborador.DadoEstagio.SetorEstagio.Id <= 0)
                        return "Setor de Estágio do colaborador inválido.";

                    var setor = new BllSetor(null).RetornarSetor(colaborador.DadoEstagio.SetorEstagio.Id);
                    if (setor == null)
                        return "Setor de Estágio do colaborador inválido.";

                    if (string.IsNullOrEmpty(colaborador.DadoEstagio.CPFSupervisor))
                        return "CPF do Supervisor do Estágio do colaborador é obrigatório.";

                    if (!ServiceUtil.EhCPFValido(colaborador.DadoEstagio.CPFSupervisor))
                        return "CPF do Supervisor do Estágio do colaborador inválido.";

                    if (string.IsNullOrEmpty(colaborador.DadoEstagio.NomeSupervisor))
                        return "Nome do Supervisor do Estágio do colaborador é obrigatório.";

                    if (!colaborador.DadoEstagio.EHorarioDinamico)
                    {
                        if (string.IsNullOrEmpty(colaborador.DadoEstagio.HorarioInicio))
                            return "Horário de Início do Estágio do colaborador é obrigatório.";

                        if (string.IsNullOrEmpty(colaborador.DadoEstagio.HorarioTermino))
                            return "Horário de Término do Estágio do colaborador é obrigatório.";

                        if (colaborador.DadoEstagio.HorarioInicioConversao() == null)
                            return "Horário de Início do Estágio do colaborador é inválido.";

                        if (colaborador.DadoEstagio.HorarioTerminoConversao() == null)
                            return "Horário de Término do Estágio do colaborador é inválido.";

                        if (colaborador.DadoEstagio.HorarioInicioConversao() > colaborador.DadoEstagio.HorarioTerminoConversao())
                            return "Horário de Início do Estágio do colaborador maior que o horário de fim.";
                    }

                }

                else
                {
                    colaborador.DadoEstagio = new DadoEstagio();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarAfastamento(Colaborador colaborador)
        {
            try
            {
                if (colaborador.Afastamentos.Any())
                {
                    colaborador.Afastamentos.Sort((a, b) => (a.DataInicioConversao().Value.CompareTo(b.DataInicioConversao().Value)));
                    for (int i = 0; i <= colaborador.Afastamentos.Count - 1; i++)
                    {
                        var afastamento = colaborador.Afastamentos[i];

                        if (afastamento.MotivoAfastamento.Id <= 0)
                            return "Motivo do Afastamento do colaborador inválido. [" + (1 + i).ToString() + "]";

                        var motivo = BllDominioFachada.BllMotivoAfastamento().Obter(afastamento.MotivoAfastamento.Id);
                        if (motivo == null)
                            return "Motivo do Afastamento do colaborador inválido. [" + (1 + i).ToString() + "]";

                        if (afastamento.DataInicioConversao() == DateTime.MinValue || afastamento.DataInicioConversao() == DateTime.MaxValue)
                            return "Data de Início do Afastamento do colaborador inválida. [" + (1 + i).ToString() + "]";

                        if (afastamento.DataInicioConversao() > DateTime.Now && afastamento.MotivoAfastamento.Id != 15)
                        {
                            //Não pode ser superior à data atual
                            return "Data de Início do Afastamento do colaborador maior que a data atual. [" + (1 + i).ToString() + "]";
                        }

                        if (afastamento.MotivoAfastamento.Id == 15 && DateTime.Now.AddDays(60) < afastamento.DataInicioConversao())
                        {
                            //se {código Motivo Afastamento} = [15] (férias), situação em que pode ser até 60 dias superior à data atual;
                            return "Data de Início do Afastamento do colaborador excedo o período de 60 dias da data atual. [" + (1 + i).ToString() + "]";
                        }

                        if (!string.IsNullOrEmpty(afastamento.DataTermino))
                        {
                            if (afastamento.DataTerminoConversao() == DateTime.MinValue || afastamento.DataTerminoConversao() == DateTime.MaxValue)
                                return "Data de Término do Afastamento do colaborador inválida. [" + (1 + i).ToString() + "]";

                            if (afastamento.DataTerminoConversao() < afastamento.DataInicioConversao())
                                return "Data de Término maior que a Data de Início do Afastamento do colaborador. [" + (1 + i).ToString() + "]";
                        }

                        //validação data fim anterior menor que a data inicio atual
                        if (i > 0)
                        {
                            var afastamentoAnterior = colaborador.Afastamentos[i - 1];

                            if (afastamentoAnterior.DataTerminoConversao() == DateTime.MinValue || afastamentoAnterior.DataInicioConversao() == DateTime.MaxValue)
                                return "Data de Fim do Afastamento anterior do colaborador inválida. [" + (1 + i).ToString() + "]";

                            if (afastamentoAnterior.DataTerminoConversao() >= afastamento.DataInicioConversao())
                                return "Data de Início do afastamento do colaborador deve ser maior que a data fim do afastamento anterior. [" + (1 + i).ToString() + "]";


                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarDadosProfissionais(Colaborador colaborador)
        {
            try
            {
                if (colaborador.ExperienciasProfissional.Any())
                {
                    int i = 0;
                    foreach (var experiencia in colaborador.ExperienciasProfissional)
                    {
                        i++;

                        //if (string.IsNullOrEmpty(experiencia.Ano.ToString()))
                        //    return "Ano da Experiência Profissional do colaborador inválido. [" + i.ToString() + "]";

                        //if (experiencia.Ano <= 0)
                        //    return "Ano da Experiência Profissional do colaborador inválido. [" + i.ToString() + "]";

                        //if (experiencia.Ano > DateTime.Now.Year)
                        //    return "Ano da Experiência Profissional do colaborador maior que o ano atual. [" + i.ToString() + "]";

                        if (string.IsNullOrEmpty(experiencia.Empresa))
                            return "Empresa da Experiência Profissional do colaborador inválido. [" + i.ToString() + "]";

                        if (string.IsNullOrEmpty(experiencia.CargoOcupado))
                            return "Cargo Ocupado da Experiência Profissional do colaborador inválido. [" + i.ToString() + "]";

                        if (string.IsNullOrEmpty(experiencia.DataInicioPeriodo.ToString()))
                            return "Data Início do Período da Experiência Profissional do colaborador inválido. [" + i.ToString() + "]";

                        if (string.IsNullOrEmpty(experiencia.DataFimPeriodo.ToString()))
                            return "Data Fim do Período da Experiência Profissional do colaborador inválido. [" + i.ToString() + "]";

                        if (experiencia.DataInicioPeriodoConversao() >= experiencia.DataFimPeriodoConversao())
                            return "Data de Início do Período da Experiência Profissional do colaborador deve ser maior que a Data fim do Período da Experiência Profissional do colaborador. [" + (1 + i).ToString() + "]";
                    }


                }


                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarCursoExtraCurricular(Colaborador colaborador)
        {
            try
            {
                if (colaborador.CursosExtraCurriculares.Any())
                {
                    int i = 0;
                    foreach (var curso in colaborador.CursosExtraCurriculares)
                    {
                        i++;

                        if (string.IsNullOrEmpty(curso.Descricao))
                            return "Descrição do Curso Extracurricular do colaborador inválida. [" + i.ToString() + "]";
                    }


                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarInstituicaoDeEnsino(Colaborador colaborador)
        {
            try
            {
                if (colaborador.TipoColaborador.Id == 1)
                {
                    if (!string.IsNullOrEmpty(colaborador.DadoEstagio.CNPJInstEnsino))
                    {
                        if (!ServiceUtil.EhCNPJValido(colaborador.DadoEstagio.CNPJInstEnsino))
                            return "CNPJ da Instituição de Ensino do colaborador inválido.";

                        if (string.IsNullOrEmpty(colaborador.DadoEstagio.RazaoSocialInstEnsino))
                            return "Razão Social da Instituição de Ensino do colaborador inválido.";

                    }
                    else
                    {
                        if (colaborador.DadoEstagio.EnderecoInstEnsino == null)
                            return "Endereço da Instituição de Ensino do colaborador é obrigatório.";

                        if (string.IsNullOrEmpty(colaborador.DadoEstagio.EnderecoInstEnsino.CEP))
                            return "CEP da Instituição de Ensino do colaborador é obrigatório.";

                        var tipoDeLogradouro = BllDominioFachada.BllTipoLogradouro().Obter(colaborador.DadoEstagio.EnderecoInstEnsino.TipoLogradouro.Id);
                        if (tipoDeLogradouro == null)
                            return "Tipo de Logradouro da Instituição de Ensino do colaborador inválido.";

                        if (string.IsNullOrEmpty(colaborador.DadoEstagio.EnderecoInstEnsino.NomeLogradouro))
                            return "Logradouro da Instituição de Ensino do colaborador é obrigatório.";

                        if (string.IsNullOrEmpty(colaborador.DadoEstagio.EnderecoInstEnsino.NumeroLogradouro))
                            colaborador.DadoEstagio.EnderecoInstEnsino.NumeroLogradouro = "S/N";

                        if (string.IsNullOrEmpty(colaborador.DadoEstagio.EnderecoInstEnsino.Bairro))
                            return "Bairro da Instituição de Ensino do colaborador é obrigatório.";

                        if (string.IsNullOrEmpty(colaborador.DadoEstagio.EnderecoInstEnsino.Municipio))
                            return "Município da Instituição de Ensino do colaborador é obrigatório.";

                        if (colaborador.DadoEstagio.EnderecoInstEnsino.UF == null)
                            return "UF da Instituição de Ensino do colaborador é obrigatório.";

                        if (string.IsNullOrEmpty(colaborador.DadoEstagio.EnderecoInstEnsino.UF.Id))
                            return "UF da Instituição de Ensino do colaborador é obrigatório.";

                        var ufDoLogradouro = new BllUnidadeFederacao().Obter(colaborador.DadoEstagio.EnderecoInstEnsino.UF.Id);
                        if (ufDoLogradouro == null)
                            return "UF da Instituição de Ensino do colaborador é obrigatório.";
                    }
                    if (!string.IsNullOrEmpty(colaborador.DadoEstagio.CNPJAgenteInt))
                    {
                        if (!ServiceUtil.EhCNPJValido(colaborador.DadoEstagio.CNPJAgenteInt))
                            return "CNPJ do Agente de Integração da Instituição de Ensino do colaborador inválido.";
                    }


                }

                else
                {
                    colaborador.DadoEstagio = new DadoEstagio();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        private string ValidarInformacaoTSVE(Colaborador colaborador)
        {
            try
            {

                if (colaborador.DadoTSVE == null)
                    return "TSVE do colaborador inválido.";

                if (colaborador.DadoTSVE.CadastramentoInicial == null)
                    return "Cadastramento Inicial TSVE do colaborador inválido.";

                if (colaborador.DadoTSVE.CadastramentoInicial.Id == 0)
                    return "Cadastramento Inicial TSVE do colaborador inválido.";

                var cadastramento = BllDominioFachada.BllCadastramentoInicial().Obter(colaborador.DadoTSVE.CadastramentoInicial.Id);
                if (cadastramento == null)
                    return "Cadastramento Inicial TSVE do colaborador inválido.";

                if (colaborador.DadoTSVE.DataInicioConversao() == DateTime.MinValue || colaborador.DadoTSVE.DataInicioConversao() == DateTime.MaxValue)
                    return "Data Início de TSVE do colaborador inválida.";

                if (colaborador.NascimentoConversao() > colaborador.DadoTSVE.DataInicioConversao())
                    return "Data Início de TSVE do colaborador deve ser maior que a data de nascimento.";

                if (colaborador.TipoColaborador.Id != 1)
                {

                    if (string.IsNullOrEmpty(colaborador.DadoTSVE.Cargo))
                        return "Cargo do TSVE do colaborador é obrigatório.";

                    if (colaborador.DadoTSVE.CBOCargo.Id == 0)
                        return "CBO Cargo do TSVE do colaborador inválido.";

                    var cboCargo = BllDominioFachada.BllCBOCargo().Obter(colaborador.DadoTSVE.CBOCargo.Id);
                    if (cboCargo == null)
                        return "CBO Cargo do TSVE do colaborador inválido.";
                }

                //Natureza da atividade é sempre 1 - URBANO
                colaborador.DadoTSVE.NaturezaAtividade.Id = 1;

                if (colaborador.TipoColaborador.Id == 1) // estagiário
                    colaborador.DadoTSVE.CategoriaTrabalhador.Id = 901;
                else //colaborador
                    colaborador.DadoTSVE.CategoriaTrabalhador.Id = 701;

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private string ValidarInformacoesAcademica(Colaborador colaborador)
        {
            try
            {
                if (colaborador.InformacoesAcademica.Count == 0)
                {
                    if (colaborador.InformacoesAcademica.Any())
                    {
                        int i = 0;
                        foreach (var InformacoesAcademica in colaborador.InformacoesAcademica)
                        {
                            i++;

                            int[] grauInstrucao = { 8, 9, 10, 11, 12 };

                            if (grauInstrucao.Contains(colaborador.GrauInstrucao.Id))
                            {
                                if (string.IsNullOrEmpty(InformacoesAcademica.Curso))
                                    return "Curso de Informações Acadêmicas do colaborador é obrigatório. [" + i.ToString() + "]";
                            }

                            if (InformacoesAcademica.Situacao.Id <= 0 )
                                return "Situação de Informações Acadêmicas do colaborador inválido. [" + i.ToString() + "]";
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        #endregion
    }
}
