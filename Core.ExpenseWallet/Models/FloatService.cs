using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class FloatService : IFloatService
    {
        private readonly IInputOutputHelper _inputOutputHelper;

        public FloatService(IInputOutputHelper inputOutputHelper)
        {
            _inputOutputHelper = inputOutputHelper;
        }
        public bool AddFloatPayment(FloatPayment payment)
        {
            var isValidAmount = SecurityUtilities.IsValidAmount($"{payment.Amount}".Replace(",","."));
            var isValidReference = SecurityUtilities.IsValidReference(payment.Reference);
            bool isValidFloatPayment = isValidAmount && isValidReference;
            if (!isValidFloatPayment) { return isValidFloatPayment; }
            var input = _inputOutputHelper.Read("Floats.json");
            var currentFloat = JsonConvert.DeserializeObject<Float>(input);
            if (currentFloat?.FloatPayments == null)
            {
                currentFloat = new Float() { FloatPayments = Default.GetDefaultFloatPayments() };
            }
            currentFloat.FloatPayments.Add(payment);

            _inputOutputHelper.Write("Floats.json", JsonConvert.SerializeObject(currentFloat));
            return isValidFloatPayment;
        }

        public Float GetFloat()
        {
            var input = _inputOutputHelper.Read("Floats.json");
            var currentFloat = JsonConvert.DeserializeObject<Float>(input);
            if (currentFloat?.FloatPayments == null)
            {
                return new Float() { FloatPayments = Default.GetDefaultFloatPayments() };
            }
            return currentFloat;
        }

        public double GetFloatBalance()
        {
            var input = _inputOutputHelper.Read("Floats.json");
            var currentFloat = JsonConvert.DeserializeObject<Float>(input);
            var floatBalance = 0.0;
            if (currentFloat?.FloatPayments == null)
            {
                return floatBalance;
            }
            floatBalance = currentFloat.FloatPayments.Select(x => x.Amount).Sum();
            return floatBalance;
        }
    }
}
