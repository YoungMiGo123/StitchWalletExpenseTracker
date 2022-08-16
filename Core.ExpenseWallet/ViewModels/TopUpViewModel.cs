using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.ViewModels
{
    public class TopUpViewModel
    {
        public string Amount { get; set; }
        public string Reference { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Message);
        public string Message { get; set; }
    }
}
