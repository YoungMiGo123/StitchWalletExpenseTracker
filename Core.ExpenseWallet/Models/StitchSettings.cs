using Core.ExpenseWallet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class StitchSettings : IStitchSettings
    {
        public string ClientId { get; set; }
        public IEnumerable<string> RedirectUrls { get ; set ; }
        public IEnumerable<string> Scopes { get ; set ; }
        public string GraphqlUrl { get; set; }
        public string AudienceUrl { get; set; }
        public string AssertionType { get; set; }
        public string GrantType { get; set; }
        public string AuthorizeUrl { get; set; }
        public string ClientSecret { get; set; }
    }
}
