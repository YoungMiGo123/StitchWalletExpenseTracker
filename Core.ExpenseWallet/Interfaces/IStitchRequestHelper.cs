using Core.ExpenseWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IStitchRequestHelper
    {
        Task<T> GetStitchResponseAsync<T>(string query, AuthenticationToken authenticationToken);
        Task<T> GetStitchResponseWithVariablesAsync<T>(string query, string jsonVariables, AuthenticationToken authenticationToken);
        AuthenticationToken GetDefaultAuthToken();
    }
}
