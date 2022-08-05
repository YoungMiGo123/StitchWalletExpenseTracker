using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class AuthenticationToken
    {
        [JsonProperty("id_token")]
        public string Id_Token { get; set; }
        [JsonProperty("access_token")]
        public string Access_Token { get; set; }
        [JsonProperty("expires_in")]
        public int Expires_In { get; set; }
        [JsonProperty("token_type")]
        public string Token_Type { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
