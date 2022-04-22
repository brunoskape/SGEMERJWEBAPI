using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using SGEMERJWEB.WebAPI.Service;
using System.Text;
using System.Web.Configuration;

[assembly: OwinStartup(typeof(SGEMERJWEB.WebAPI.Startup))]

namespace SGEMERJWEB.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            //essa classe que faz o token ser usado e validado

            //string key_jwt = WebConfigurationManager.AppSettings["key_jwt"];
            string key_jwt = TokenService.GetKeyJWT();
            string issuer_jwt = WebConfigurationManager.AppSettings["issuer_jwt"];

            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer_jwt, //some string, normally web url,  
                        ValidAudience = issuer_jwt,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key_jwt))
                    }
                });
        }
    }
}
