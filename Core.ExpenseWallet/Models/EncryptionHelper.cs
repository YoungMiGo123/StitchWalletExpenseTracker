using Core.ExpenseWallet.Interfaces;
using Newtonsoft.Json;

namespace Core.ExpenseWallet.Models
{
    public class EncryptionHelper : IEncryptionHelper
    {
        private readonly IInputOutputHelper _inputOutputHelper;
        private readonly IHttpService _httpService;
        private readonly IStitchSettings stitchSettings;

        public EncryptionHelper(IInputOutputHelper inputOutputHelper, IHttpService httpService, IStitchSettings stitchSettings)
        {
            _inputOutputHelper = inputOutputHelper;
            _httpService = httpService;
            this.stitchSettings = stitchSettings;
        }
        public async Task<AuthModel> GetAuthModel(bool useExistingValues = false)
        {
            if (useExistingValues)
            {
                var json = _inputOutputHelper.Read(SecurityUtilities.StitchSettingsJsonPath);
                var auth = JsonConvert.DeserializeObject<AuthModel>(json);
                return auth;
            }
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
            await SaveAuth(authModel);

            return authModel;
        }
        private async Task SaveAuth(AuthModel authModel)
        {
            _inputOutputHelper.Write(SecurityUtilities.StitchSettingsJsonPath, JsonConvert.SerializeObject(authModel));
            var url = stitchSettings.RedirectUrls.Last().Replace("return", "UpdateAuthModel");
            await _httpService.Post<bool>(url, JsonConvert.SerializeObject(authModel));
        }
    }
}
