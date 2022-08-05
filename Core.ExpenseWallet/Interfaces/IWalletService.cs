using Core.ExpenseWallet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IWalletService
    {
        Task<ExpenseWalletView> GetExpenseWalletView(string code);
        Task<TransactionCategoryView> GetTransactionCategoryView();
    }
}
