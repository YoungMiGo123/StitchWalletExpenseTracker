using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class RedirectUrlModel
    {
        public string RedirectUrl { get; set; }
        public string AuthorizationUrl { get; set; }
        public bool UseExistingAuthModel { get; set; }
    }
}
