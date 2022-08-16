using Core.ExpenseWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IFloatService
    {
        public Float GetFloat();
        public double GetFloatBalance();
        public bool AddFloatPayment(FloatPayment payment);
    }
}
