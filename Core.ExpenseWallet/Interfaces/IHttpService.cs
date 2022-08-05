using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IHttpService
    {
        Task<T> GetGraphqlResponseAsync<T>(string url, string jsonRequest, Dictionary<string, string> headers = null);
        Task<T> PostWithBody<T>(string url, string jsonRequest);
    }
}
