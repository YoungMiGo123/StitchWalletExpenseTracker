using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class AuthModel
    {
        public string Challenge { get; set; }
        public string AuthenticationUrl { get; set; }
        public string State { get; set; }
        public string Nonce { get; set; }
        public string Verifier { get; set; }
    }
}
