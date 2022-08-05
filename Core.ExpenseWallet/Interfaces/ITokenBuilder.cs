using Core.ExpenseWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface ITokenBuilder
    {
        Task<AuthenticationToken> GetTokenWithCode(string code);
    }
}
