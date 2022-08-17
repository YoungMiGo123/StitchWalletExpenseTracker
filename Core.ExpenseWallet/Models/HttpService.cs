using Core.ExpenseWallet.Interfaces;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

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

        public async Task<T> GetGraphqlResponseAsyncWithVariables<T>(string url, string jsonRequest, string jsonVariables, Dictionary<string, string> headers = null)
        {
            try
            {
                var stitchRequest = new GraphQLRequest
                {
                    Query = jsonRequest,
                    Variables = jsonVariables
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
        public async Task<T> Post<T>(string Url, string json, Dictionary<string, string> Headers = null)
        {
            try
            {

                var request = new HttpRequestMessage();
                if (Headers != null)
                {
                    request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(Url),
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                        Headers =
                        {
                                 { HttpRequestHeader.Authorization.ToString(), Headers.FirstOrDefault().Value },
                                 { HttpRequestHeader.Accept.ToString(), "application/json" },
                        }
                    };
                }
                else
                {
                    request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(Url),
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                }
                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(responseString);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogInformation($"An error occured while processing this - {e}");
                return default;
            }
        }

        public async Task<T> Get<T>(string Url, Dictionary<string, string> Headers = null)
        {
            try
            {
                if (Headers != null)
                {
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(Url),
                        Headers =
                        {
                            { HttpRequestHeader.Authorization.ToString(), Headers.FirstOrDefault().Value },
                            { HttpRequestHeader.Accept.ToString(), "application/json" },
                        }
                    };

                    var responseMessage = await _httpClient.SendAsync(httpRequestMessage);
                    if (responseMessage != null && responseMessage.IsSuccessStatusCode)
                    {
                        var response = await responseMessage.Content.ReadAsStringAsync();
                        var resultWithHeaders = JsonConvert.DeserializeObject<T>(response);
                        return resultWithHeaders;
                    }

                }

                var responseString = await _httpClient.GetStringAsync(Url);
                var result = JsonConvert.DeserializeObject<T>(responseString);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogInformation($"An error occured while processing this - {e}");
                return default;
            }
        }
    }
}
