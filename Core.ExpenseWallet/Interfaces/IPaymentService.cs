using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Models;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IPaymentService
    {
        Task<StitchResponse> GetPaymentInitiation(FloatPayment payment);
        string GetMultifactorPaymentIniationUrl(StitchResponse stitchResponse);

    }
}
