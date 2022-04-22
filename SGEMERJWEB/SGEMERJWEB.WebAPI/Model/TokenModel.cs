using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGEMERJWEB.WebAPI.Model
{
    public class TokenModel
    {
        public string TokenJWT { get; set; }
        public DateTime Expiration { get; set; }          
    }
}