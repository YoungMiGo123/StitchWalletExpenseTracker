using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class FloatService : IFloatService
    {
        private readonly IInputOutputHelper _inputOutputHelper;
        private readonly IStitchSettings _stitchSettings;
        private readonly ITokenBuilder _tokenBuilder;
        private readonly IStitchRequestHelper _stitchRequestHelper;
        private readonly IUrlService _urlService;
        private readonly IHttpService _httpService;

        public FloatService(IInputOutputHelper inputOutputHelper, IStitchSettings stitchSettings, ITokenBuilder tokenBuilder, IStitchRequestHelper stitchRequestHelper, IUrlService urlService,IHttpService httpService)
        {
            _tokenBuilder = tokenBuilder;
            _stitchRequestHelper = stitchRequestHelper;
            _urlService = urlService;
            _httpService = httpService;
            _inputOutputHelper = inputOutputHelper;
            _stitchSettings = stitchSettings;
        }
        public Float GetFloat()
        {
            var currentFloat = GetCurrentFloat();
            return currentFloat;
        }

        public double GetFloatBalance()
        {
            var input = _inputOutputHelper.Read(SecurityUtilities.FloatsJsonPath);
            var currentFloat = JsonConvert.DeserializeObject<Float>(input);
            var floatBalance = 0.0;
            if (currentFloat?.FloatPayments == null)
            {
                return floatBalance;
            }
            floatBalance = currentFloat.FloatPayments.Select(x => x.Amount).Sum();
            return floatBalance;
        }

        public async Task<string> GetPaymentAuthorizationUrl()
        {
            var clientToken = await _tokenBuilder.GetClientToken();
            _inputOutputHelper.Write(SecurityUtilities.ClientTokenJsonPath, JsonConvert.SerializeObject(clientToken));
            var linkingRequest = await _stitchRequestHelper.GetStitchResponseAsync<StitchResponse>(GraphqlQueries.CreateAccountLinkingRequest, clientToken);
            var authorizationRequestUrl = linkingRequest.data.clientPaymentAuthorizationRequestCreate.authorizationRequestUrl;
            return authorizationRequestUrl;
        }

        private PaymentInitiationStatus GetPaymentInitiationStatus(string status)
        {
            switch (status)
            {
                case "PaymentInitiationCompleted": return PaymentInitiationStatus.PaymentInitiationCompleted;
                case "USER_INTERACTION_REQUIRED": return PaymentInitiationStatus.PaymentInitiationPending;
                default: return PaymentInitiationStatus.PaymentInitiationFailed;
            }
        }
        private bool isValidFloatPayment(FloatPayment payment)
        {
            var isValidAmount = SecurityUtilities.IsValidAmount($"{payment.Amount}".Replace(",", "."));
            var isValidReference = SecurityUtilities.IsValidReference(payment.Reference);
            bool isValidFloatPayment = isValidAmount && isValidReference;
            return isValidFloatPayment;
        }

        private async Task<PaymentInitiation> GetPaymentInitiation(FloatPayment payment, AuthenticationToken authenticationToken)
        {
            var paymentVars = new
            {
                amount = new { quantity = payment.Amount, currency = payment.Currency },
                payerReference = payment.Reference,
                externalReference = payment.Reference
            };
            var defaultToken = authenticationToken;
            var paymentInitiationResponse = await _stitchRequestHelper.GetStitchResponseWithVariablesAsync<StitchResponse>(GraphqlQueries.UserInitiatePayment, JsonConvert.SerializeObject(paymentVars), defaultToken);
            var paymentInitiationResult = paymentInitiationResponse.data.userInitiatePayment.paymentInitiation;
            return paymentInitiationResult;
        }
        private Float GetCurrentFloat()
        {
            var input = _inputOutputHelper.Read(SecurityUtilities.FloatsJsonPath);
            var currentFloat = JsonConvert.DeserializeObject<Float>(input);
            if (currentFloat?.FloatPayments == null)
            {
                currentFloat = new Float() { FloatPayments = Default.GetDefaultFloatPayments() };
            }
            return currentFloat;
        }

        public async Task<FloatPayment> AddFloatPayment(FloatPayment payment)
        {
            if (!isValidFloatPayment(payment)) { return payment; }
            var url = _stitchSettings.RedirectUrls.Last().Replace("return", "GetAuthenticationToken");
            var authToken = await _httpService.Get<AuthenticationToken>(url);
            var paymentInitiationResult = await GetPaymentInitiation(payment, authToken);
            if (paymentInitiationResult != null)
            {
                var floatPayment = new FloatPayment
                {
                    Amount = Convert.ToDouble(paymentInitiationResult.amount.quantity, CultureInfo.InvariantCulture),
                    Balance = payment.Balance,
                    CreatedDate = paymentInitiationResult.date,
                    Currency = payment.Currency,
                    Id = payment.Id,
                    Reference = payment.Reference,
                    Status = GetPaymentInitiationStatus(paymentInitiationResult.status.__typename)
                };
                var currentFloat = GetCurrentFloat();
                currentFloat.AddFloatPayment(floatPayment);
                _inputOutputHelper.Write(SecurityUtilities.FloatsJsonPath, JsonConvert.SerializeObject(currentFloat));
            }
            return payment;
        }
    }
}
