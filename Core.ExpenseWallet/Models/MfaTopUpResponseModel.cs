using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Core.ExpenseWallet.Models
{
    public class MfaTopUpResponseModel
    {
        [FromQuery]
        public string id { get; set; }
        [FromQuery]
        public string status { get; set; }
        [FromQuery]
        public string externalReference { get; set; }
        public PaymentInitiationStatus paymentInitiationStatus => PaymentUtilities.GetPaymentInitiationStatus(status);
    }
}
