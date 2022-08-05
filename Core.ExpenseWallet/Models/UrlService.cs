using Core.ExpenseWallet.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Core.ExpenseWallet.Models
{
    public class UrlService : IUrlService
    {

        private readonly IInputOutputHelper _inputOutputHelper;
        private readonly IStitchSettings _stitchSettings;
        public UrlService(IStitchSettings stitchSettings, IInputOutputHelper inputOutputHelper)
        {
            _inputOutputHelper = inputOutputHelper;
            _stitchSettings = stitchSettings;
        }
        public async Task<string> BuildUrl()
        {
            var challenge = await SecurityUtilities.GenerateVerifierChallengePair();
            var state = SecurityUtilities.GetStateOrNonce();
            var nonce = SecurityUtilities.GetStateOrNonce();
            var authModel = new AuthModel()
            {
                Challenge = challenge.Challenge,
                Nonce = nonce,
                State = state,
                Verifier = challenge.Verfier
            };
            var url = GetUrl(authModel);
            authModel.AuthenticationUrl = url;
            _inputOutputHelper.Write(SecurityUtilities.JsonFilePath, JsonConvert.SerializeObject(authModel));
            return url;
        }
        private string GetUrl(AuthModel authModel)
        {
            var search = new Dictionary<string, string>
            {
                { "client_id", _stitchSettings.ClientId },
                { "code_challenge", authModel.Challenge },
                { "code_challenge_method", "S256" },
                { "redirect_uri", _stitchSettings.RedirectUrl },
                { "scope", string.Join(" ", _stitchSettings.Scopes) },
                { "response_type", "code" },
                { "nonce", authModel.Nonce },
                { "state", authModel.State }
            };
            var searchString = string.Join("&", search.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            return $"https://secure.stitch.money/connect/authorize?{searchString}";
        }
    }
}
