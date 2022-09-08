using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Models;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IFloatService
    {
        public Task<string> GetPaymentAuthorizationUrl();
        public Float GetFloat();
        public double GetFloatBalance();
        public Task<FloatPayment> AddFloatPayment(FloatPayment payment, StitchResponse stitchResponse);
        public FloatPayment AddFloatPayment(MfaTopUpResponseModel mfaTopUpResponseModel);
    }
}
