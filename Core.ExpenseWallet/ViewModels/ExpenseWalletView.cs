using Core.ExpenseWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.ViewModels
{
    public class ExpenseWalletView
    {
        public List<BankAccount> BankAccounts { get; set; }
        public List<Node> Transactions { get; set; }
        public List<Node> DebitOrders { get; set; }
        public List<Node> SalaryInformation { get; set; }
        public Income Income { get; set; }
    }
}
