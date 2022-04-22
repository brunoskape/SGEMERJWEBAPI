using SGEMERJWEB.Dal;
using SGEMERJWEB.Entidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGEMERJWEB.Bll
{
    public class BllTipoBeneficio
    {
        private readonly DalTipoBeneficio dalTipoBeneficio;

        public BllTipoBeneficio(string codusu)
        {
            dalTipoBeneficio = new DalTipoBeneficio(codusu);
        }

        public string GravarTipoBeneficio(TipoBeneficio tipoBeneficio)
        {
            try
            {
                if (tipoBeneficio != null)
                {
                        if (dalTipoBeneficio.BeneficioExiste(tipoBeneficio.Id, tipoBeneficio.Descricao))
                        return "O Benefício de pagamento já existe.";

                    return dalTipoBeneficio.GravarTipoBeneficio(tipoBeneficio);
                }
                else
                {
                    throw new ArgumentNullException(nameof(tipoBeneficio), "Dados do tipo de benefício não informado!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString(), ex.InnerException);
            }
        }
        public (int, int, int, IEnumerable<TipoBeneficio>) RetornarTipoBeneficios(string d, int p)
        {
            try
            {
                int pagina = p;
                const int maxLista = 10;

                if (pagina != 0)
                {
                    pagina -= 1;
                }

                var tipoBeneficio = new TipoBeneficio
                {
                    Descricao = d
                };

                var lstTipoBeneficio = dalTipoBeneficio.RetornarTipoBeneficios(tipoBeneficio);
                var totRegistro = lstTipoBeneficio.Count();
                var totPagina = (int)Math.Ceiling((totRegistro / (decimal)maxLista));

                if (pagina > totPagina)
                {
                    pagina = 0;
                }

                var lstTipoBeneficioPg = lstTipoBeneficio.Skip(pagina * maxLista)
                         .Take(maxLista).ToList();

                pagina++;

                return (pagina, totPagina, totRegistro, lstTipoBeneficioPg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public IEnumerable<TipoBeneficio> RetornarTipoBeneficioesAtivos()
        {
            try
            {
              
                var lstTipoBeneficio = dalTipoBeneficio.RetornartipoBeneficiosAtivos();
                return lstTipoBeneficio;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public TipoBeneficio RetornarTipoBeneficio(int id)
        {
            try
            {
                var tipoBeneficio = dalTipoBeneficio.RetornarTipoBeneficio(id);
                return tipoBeneficio;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
