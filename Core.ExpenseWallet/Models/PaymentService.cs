using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Utilities;
using Newtonsoft.Json;
using System.Net;

namespace Core.ExpenseWallet.Models
{
    public class PaymentService : IPaymentService
    {
        private readonly IStitchRequestHelper _stitchRequestHelper;
        private readonly IHttpService _httpService;
        private readonly IStitchSettings _stitchSettings;
        private readonly IInputOutputHelper _inputOutputHelper;

        public PaymentService(IStitchRequestHelper stitchRequestHelper, IHttpService httpService, IStitchSettings stitchSettings, IInputOutputHelper inputOutputHelper)
        {
            _stitchRequestHelper = stitchRequestHelper;
            _httpService = httpService;
            _stitchSettings = stitchSettings;
            _inputOutputHelper = inputOutputHelper;
        }
        public async Task<StitchResponse> GetPaymentInitiation(FloatPayment payment)
        {
            var url = _stitchSettings.RedirectUrls.Last().Replace("return", "GetAuthenticationToken");
            var authToken = await _httpService.Get<AuthenticationToken>(url);
            var paymentVars = new
            {
                amount = new { quantity = payment.Amount, currency = payment.Currency },
                payerReference = payment.Reference,
                externalReference = payment.Reference
            };
            var stitchResponse = await _stitchRequestHelper.GetStitchResponseWithVariablesAsync<StitchResponse>(GraphqlQueries.UserInitiatePayment, JsonConvert.SerializeObject(paymentVars), authToken);
            if (stitchResponse.HasErrors)
            {
                _inputOutputHelper.Write(SecurityUtilities.MfaRequiredTopUps, JsonConvert.SerializeObject(payment));
            }
            return stitchResponse;
        }

        public string GetMultifactorPaymentIniationUrl(StitchResponse stitchResponse)
        {
            if (stitchResponse == null || !stitchResponse.HasErrors)
            {
                return string.Empty;
            }
            var url = WebUtility.UrlEncode(_stitchSettings.RedirectUrls.First().Replace("return", "HandleMfaTopUp"));
            var redirectUrl = stitchResponse?.Errors?.FirstOrDefault()?.Extensions.userInteractionUrl ?? string.Empty;
            redirectUrl = string.IsNullOrEmpty(redirectUrl) ? string.Empty : $"{redirectUrl}?redirect_uri={url}";
            return redirectUrl;
        }
    }
}
