using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IHttpService
    {
        Task<T> Get<T>(string Url, Dictionary<string, string> Headers = null);
        Task<T> GetGraphqlResponseAsync<T>(string url, string jsonRequest, Dictionary<string, string> headers = null);
        Task<T> GetGraphqlResponseAsyncWithVariables<T>(string url, string jsonRequest, string jsonVariables, Dictionary<string, string> headers = null);
        Task<T> Post<T>(string Url, string json, Dictionary<string, string> Headers = null);
        Task<T> PostWithBody<T>(string url, string jsonRequest);
    }
}
