using Core.ExpenseWallet.Interfaces;
using GraphQL;
using GraphQL.Client.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Core.ExpenseWallet.Models
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> _logger;

        public HttpService(ILogger<HttpService> logger)
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
            _logger = logger;
        }
        public async Task<T> GetGraphqlResponseAsync<T>(string url, string jsonRequest, Dictionary<string, string> headers = null)
        {
            try
            {
                var stitchRequest = new GraphQLRequest
                {
                    Query = jsonRequest
                };
                var graphqlHttpOptions = new GraphQLHttpClientOptions { EndPoint = new Uri(url) };
                if (headers != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(headers.Keys.FirstOrDefault(), headers.Values.FirstOrDefault());
                }
                var grahpqlClient = new GraphQLHttpClient(graphqlHttpOptions, new NewtonsoftJsonSerializer(), _httpClient);
                var graphqlResponse = await grahpqlClient.SendQueryAsync<dynamic>(stitchRequest);
                var graphqlStringResponse = JsonConvert.SerializeObject(graphqlResponse);
                var graphqlResult = JsonConvert.DeserializeObject<T>(graphqlStringResponse);
                return graphqlResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while getting the response object. Exception = {ex}");
                return default(T);
            }
        }

        public async Task<T> PostWithBody<T>(string url, string jsonRequest)
        {
            try
            {
                var webClient = new WebClient();
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] byteResponse = webClient.UploadData(url, "POST", Encoding.ASCII.GetBytes(jsonRequest));
                string stringResult = Encoding.ASCII.GetString(byteResponse);
                var result = JsonConvert.DeserializeObject<T>(stringResult);
                return await Task.Run(() => result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while getting the response object. Exception = {ex}");
                return await Task.Run(() => default(T));
            }
        }
    }
}
