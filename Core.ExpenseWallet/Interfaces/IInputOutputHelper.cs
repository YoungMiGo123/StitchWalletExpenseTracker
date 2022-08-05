using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Interfaces
{
    public interface IInputOutputHelper
    {
        string Read(string path);
        void Write(string path, string values);
    }
}
