using Core.ExpenseWallet.Data;

namespace Core.ExpenseWallet.Utilities
{
    public static class PaymentUtilities
    {
        public static PaymentInitiationStatus GetPaymentInitiationStatus(string status)
        {
            switch (status)
            {
                case "PaymentInitiationCompleted":
                case "complete": 
                    return PaymentInitiationStatus.PaymentInitiationCompleted;
                case "USER_INTERACTION_REQUIRED": return PaymentInitiationStatus.PaymentInitiationPending;
               
                default: return PaymentInitiationStatus.PaymentInitiationFailed;
            }
        }
    }
}
