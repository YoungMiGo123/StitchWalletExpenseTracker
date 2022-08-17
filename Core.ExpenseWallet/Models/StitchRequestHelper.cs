using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class StitchRequestHelper : IStitchRequestHelper
    {
        private readonly IStitchSettings _stitchSettings;
        private readonly IHttpService _httpService;
        private readonly IInputOutputHelper _inputOutputHelper;

        public StitchRequestHelper(IStitchSettings stitchSettings, IHttpService httpService, IInputOutputHelper inputOutputHelper)
        {
            _stitchSettings = stitchSettings;
            _httpService = httpService;
            _inputOutputHelper = inputOutputHelper;
        }
        public async Task<T> GetStitchResponseAsync<T>(string query, AuthenticationToken authenticationToken)
        {
            if (authenticationToken == null)
            {
                authenticationToken = GetDefaultAuthToken();
            }
            var request = new StitchRequest
            {
                AuthenticationToken = authenticationToken,
                Query = query
            };
            var headers = new Dictionary<string, string>()
            {
                {$"{Default.HeaderType.Bearer}",request.AuthenticationToken.Access_Token }
            };
            var response = await _httpService.GetGraphqlResponseAsync<T>(_stitchSettings.GraphqlUrl, request.Query, headers);
            return response;
        }
       
        public AuthenticationToken GetDefaultAuthToken()
        {
            var tokenString = _inputOutputHelper.Read(SecurityUtilities.UserTokenJsonPath);
            var token = JsonConvert.DeserializeObject<AuthenticationToken>(tokenString);
            return token;
        }
        public async Task<T> GetStitchResponseWithVariablesAsync<T>(string query, string jsonVariables, AuthenticationToken authenticationToken)
        {
            if (authenticationToken == null)
            {
                authenticationToken = GetDefaultAuthToken();
            }
            var request = new StitchRequest
            {
                AuthenticationToken = authenticationToken,
                Query = query
            };
            var headers = new Dictionary<string, string>()
            {
                {$"{Default.HeaderType.Bearer}",request.AuthenticationToken.Access_Token }
            };
            var response = await _httpService.GetGraphqlResponseAsyncWithVariables<T>(_stitchSettings.GraphqlUrl, request.Query,jsonVariables, headers);
            return response;
        }
    }
}
