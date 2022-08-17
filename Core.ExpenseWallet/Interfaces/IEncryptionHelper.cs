using Core.ExpenseWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IEncryptionHelper
    {
        public Task<AuthModel> GetAuthModel(bool useExistingValues = false);
    }
}
