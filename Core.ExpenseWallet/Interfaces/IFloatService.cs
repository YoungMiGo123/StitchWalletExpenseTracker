using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IFloatService
    {
        public Task<string> GetPaymentAuthorizationUrl();
        public Float GetFloat();
        public double GetFloatBalance();
        public Task<FloatPayment> AddFloatPayment(FloatPayment payment);
    }
}
