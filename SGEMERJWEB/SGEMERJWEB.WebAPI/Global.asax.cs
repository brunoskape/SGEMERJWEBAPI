using Microsoft.AspNet.WebApi.Extensions.Compression.Server;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SGEMERJWEB.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {            
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            //força retorno json para todas as respostas
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());            

            //comprimir json response
            GlobalConfiguration.Configuration.MessageHandlers.Insert(0, new ServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));

            //para não devolver xml senão dá erro no token
            //GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);

            //formatar o json de resposta para camelCase (minuscula e maiusculas)
            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        /*
          Web API precisa responder à Options solicitação para confirmar que está realmente configurado 
          para oferecer suporte CORS.
          Para lidar com isso, enviamos uma resposta vazia de volta.
          Fonte: https://stackoverflow.com/questions/27504256/mvc-web-api-no-access-control-allow-origin-header-is-present-on-the-requested
        */
        protected void Application_BeginRequest()
        {
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.Flush();
            }
        }
    }
}
