using Core.ExpenseWallet.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Core.ExpenseWallet.Models
{
    public class UrlService : IUrlService
    {

 
        private readonly IStitchSettings _stitchSettings;
        private readonly IEncryptionHelper _encryption;

        public UrlService(IStitchSettings stitchSettings, IEncryptionHelper encryption)
        {
            _stitchSettings = stitchSettings;
            _encryption = encryption;
        }
        public async Task<string> BuildUrl(RedirectUrlModel redirectUrlModel)
        {
            var authModel = await _encryption.GetAuthModel(redirectUrlModel.UseExistingAuthModel);
            var redirectUrl = string.IsNullOrEmpty(redirectUrlModel.RedirectUrl) ? _stitchSettings.RedirectUrls.First() : redirectUrlModel.RedirectUrl;
            var url = GetUrl(redirectUrlModel.AuthorizationUrl, redirectUrl, authModel);
            authModel.AuthenticationUrl = url;
            return url;
        }
        private string GetUrl(string authorizationUrl, string redirectUrl, AuthModel authModel)
        {
            var search = new Dictionary<string, string>
            {
                { "client_id", _stitchSettings.ClientId },
                { "code_challenge", authModel.Challenge },
                { "code_challenge_method", "S256" },
                { "redirect_uri", redirectUrl },
                { "scope", string.Join(" ", _stitchSettings.Scopes) },
                { "response_type", "code" },
                { "nonce", authModel.Nonce },
                { "state", authModel.State }
            };
            var searchString = string.Join("&", search.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            return $"{authorizationUrl}?{searchString}";
        }
    }
}
