using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IStitchSettings
    {
        public string ClientId { get; set; }
        public string RedirectUrl { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public string GraphqlUrl { get; set; }
        public string AudienceUrl { get; set; }
        public string AssertionType { get; set; }
        public string GrantType { get; set; }
    }
}
