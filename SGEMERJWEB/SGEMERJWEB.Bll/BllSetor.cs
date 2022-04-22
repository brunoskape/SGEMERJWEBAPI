using SGEMERJWEB.Dal;
using SGEMERJWEB.Entidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGEMERJWEB.Bll
{
    public class BllSetor
    {
        private DalSetor dalSetor;

        public BllSetor(string codusu)
        {
            dalSetor = new DalSetor(codusu);
        }

        public string GravarSetor(Setor setor)
        {
            try
            {
                if (setor != null)
                {
                        if (dalSetor.SiglaExiste(setor.Id, setor.Sigla))
                        return "Sigla do setor já existe.";

                    return dalSetor.GravarSetor(setor);
                }
                else
                {
                    throw new ArgumentNullException(nameof(setor), "Dados do Setor não informado!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString(), ex.InnerException);
            }
        }
        public (int, int, int, IEnumerable<Setor>) RetornarSetores(string d, string s, int p)
        {
            try
            {
                int pagina = p;
                const int maxLista = 10;

                if (pagina != 0)
                {
                    pagina -= 1;
                }

                var setor = new Setor
                {
                    Descricao = d,
                    Sigla = s
                };

                var lstSetor = dalSetor.RetornarSetores(setor);
                var totRegistro = lstSetor.Count();
                var totPagina = (int)Math.Ceiling((totRegistro / (decimal)maxLista));

                if (pagina > totPagina)
                {
                    pagina = 0;
                }

                var lstSetorPg = lstSetor.Skip(pagina * maxLista)
                         .Take(maxLista).ToList();

                pagina++;

                return (pagina, totPagina, totRegistro, lstSetorPg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public IEnumerable<Setor> RetornarSetoresAtivos()
        {
            try
            {
              
                var lstSetor = dalSetor.RetornarSetoresAtivos();
                return lstSetor;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public Setor RetornarSetor(int id)
        {
            try
            {
                var setor = dalSetor.RetornarSetor(id);
                return setor;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
