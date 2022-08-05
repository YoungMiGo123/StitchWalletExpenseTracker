using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class AuthenticationResponseModel
    {
        [FromQuery]
        [JsonProperty("code")]
        public string Code { get; set; }
        [FromQuery]
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [FromQuery]
        [JsonProperty("state")]
        public string State { get; set; }
        
        [FromQuery]
        [JsonProperty("session_state")]
        public string Session_State { get; set; }
    }
}
