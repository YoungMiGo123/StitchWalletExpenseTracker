using Core.ExpenseWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Utilities
{
    public static class Default
    {
        public static List<Node> GetSalaryInfo
        {
            get => new List<Node>()
                {
                    new Node(){
                        frequency = "fortnightly",
                        nextSalaryPaymentExpectedDate = Convert.ToDateTime("2022-08-14T00:00:00.000Z"),
                        nextSalaryPaymentStandardDeviationInDays = "8.588752334691382092",
                        previousSalaryPaymentDate = Convert.ToDateTime("2022-07-31T00:00:00.000Z"),
                        salaryExpected = new SalaryExpected {currency = "ZAR", quantity = "32000"}
                    }
                };
        }
        public static Income Income
        {
            get => new Income()
            {
                totalIncome = new TotalIncome { quantity = "355937.68", currency = "ZAR" }
            };
        }
        public enum HeaderType
        {
            Bearer = 1
        }
        public static List<FloatPayment> GetDefaultFloatPayments()
        {
            return new List<FloatPayment>();
          
        }
       
        public static string DefaultCurrency => "ZAR";
        public static string DefaultTopUpErrorMessage => "Something went wrong during the top up, please try again ?";
    }
}
