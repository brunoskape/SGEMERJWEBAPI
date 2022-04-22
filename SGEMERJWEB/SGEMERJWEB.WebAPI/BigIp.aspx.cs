using System;

namespace SGEMERJWEB.WebAPI
{
    public partial class BigIp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var nomeMaq = Environment.MachineName.ToUpper();
                var bigip = Request["CODBIGIP"] ?? "";

                Response.Write("Nome Servidor: " + nomeMaq + "<br /> Health Check: " + bigip);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }
    }
}