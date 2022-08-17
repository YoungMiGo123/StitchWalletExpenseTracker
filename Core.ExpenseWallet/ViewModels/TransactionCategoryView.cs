using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Models;

namespace Core.ExpenseWallet.ViewModels
{
    public class TransactionCategoryView
    {
        public IEnumerable<BankAccount> BankAccounts { get;  set; }
        public IEnumerable<TransactionCategory> TopSpendingCategories { get;  set; }
    }
}