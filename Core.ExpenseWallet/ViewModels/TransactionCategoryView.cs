using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Models;

namespace Core.ExpenseWallet.ViewModels
{
    public class TransactionCategoryView
    {
        public IEnumerable<BankAccount> BankAccounts { get; internal set; }
        public IEnumerable<TransactionCategory> TopSpendingCategories { get; internal set; }
    }
}