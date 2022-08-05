using Core.ExpenseWallet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class IOHelper : IInputOutputHelper
    {
        public string Read(string path)
        {
            var result = File.ReadAllText(path);
            return result;
        }

        public void Write(string path, string values)
        {
            File.WriteAllText(path, values);
        }
    }
}
