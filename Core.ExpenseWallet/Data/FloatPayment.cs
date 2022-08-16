using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Data
{
    public class FloatPayment
    {
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public double Balance { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Currency { get;set; }
    }
    public class Float
    {
       public List<FloatPayment> FloatPayments { get; set; }
       
    }
}
