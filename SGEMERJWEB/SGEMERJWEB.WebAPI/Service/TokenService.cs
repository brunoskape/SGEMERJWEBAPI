using Microsoft.IdentityModel.Tokens;
using SGEMERJWEB.WebAPI.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;

namespace SGEMERJWEB.WebAPI.Service
{
    public class TokenService
    {
        static private string KeyJWT = null;
        public TokenModel Get(string codusu, string idusu)
        {
            try
            {
                //string key_jwt = WebConfigurationManager.AppSettings["key_jwt"];
                string key_jwt = GetKeyJWT();
                string issuer_jwt = WebConfigurationManager.AppSettings["issuer_jwt"];
                string minute_jwt = WebConfigurationManager.AppSettings["minute_jwt"];

                int interval = 1;

                if (!string.IsNullOrEmpty(minute_jwt))
                    if (int.Parse(minute_jwt) > 1)
                        interval = int.Parse(minute_jwt);



                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key_jwt));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                DateTime expire = DateTime.Now.AddMinutes(interval);

                var permClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("expire", expire.ToString()),
                    new Claim("codusu", codusu),
                    new Claim("idusu", idusu )
                };

                var token = new JwtSecurityToken(issuer_jwt, //Issure    
                                issuer_jwt,  //Audience    
                                permClaims,
                                expires: expire,
                                signingCredentials: credentials);
                var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

                var tokenModel = new TokenModel
                {
                    TokenJWT = jwt_token,
                    Expiration = expire,
                };

                return tokenModel;
            }
            catch
            {
                return null;
            }
        }

        static public string GetKeyJWT()
        {
            if (!string.IsNullOrEmpty(KeyJWT))
                return KeyJWT;

            var hmac = new HMACSHA256();
            KeyJWT = Convert.ToBase64String(hmac.Key);
           
            return KeyJWT;
        }


        static public string GetCodUsu(ClaimsIdentity identity)
        {
            try
            {
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var dado = claims.Where(p => p.Type == "codusu").FirstOrDefault()?.Value;
                    return dado;

                }
                return Environment.UserName;
            }
            catch
            {
                return Environment.UserName;
            }

        }

        static public string GetIdUsu(ClaimsIdentity identity)
        {
            try
            {
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var dado = claims.Where(p => p.Type == "idusu").FirstOrDefault()?.Value;
                    return dado;

                }
                return Environment.UserName;
            }
            catch
            {
                return Environment.UserName;
            }

        }
    }
}