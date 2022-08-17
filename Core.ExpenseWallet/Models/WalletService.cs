using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Utilities;
using Core.ExpenseWallet.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Models
{
    public class WalletService : IWalletService
    {
        private readonly ITokenBuilder _tokenBuilder;
        private readonly IHttpService _httpService;
        private readonly IStitchSettings _stitchSettings;
        private readonly IInputOutputHelper _inputOutputHelper;
        private readonly IFloatService _floatService;
        private readonly IStitchRequestHelper _stitchRequestHelper;

        public WalletService(ITokenBuilder tokenBuilder, IHttpService httpService, IStitchSettings stitchSettings, IInputOutputHelper inputOutputHelper, IFloatService floatService,
            IStitchRequestHelper stitchRequestHelper)
        {
            _tokenBuilder = tokenBuilder;
            _httpService = httpService;
            _stitchSettings = stitchSettings;
            _inputOutputHelper = inputOutputHelper;
            _floatService = floatService;
            _stitchRequestHelper = stitchRequestHelper;
        }
  
        public async Task<ExpenseWalletView> GetExpenseWalletView(string code)
        {
            AuthenticationToken token;
            if (string.IsNullOrEmpty(code)) 
            {
                token = _stitchRequestHelper.GetDefaultAuthToken();
                if(token == null) { throw new Exception("No code provided to get the token"); }
            }
            else
            {
                token = await _tokenBuilder.GetTokenWithCode(code);
            }

            await SaveUserToken(token);
            var listBankAccounts = await _stitchRequestHelper.GetStitchResponseAsync<StitchResponse>(GraphqlQueries.ListBankAccountTransactions, token);
            var listBankAcountEdges = listBankAccounts.data.user.bankAccounts.Where(x => x.transactions.edges.Any()).SelectMany(x => x.transactions.edges).Select(x => x.node).ToList();
            var debitOrders = await GetDebitOrders(GraphqlQueries.ListOfDebitOrders, token);
            var salaryInfo = await _stitchRequestHelper.GetStitchResponseAsync<StitchResponse>(GraphqlQueries.SalaryInformation, token);
            var salaryInfoNodes = salaryInfo.data?.user?.salaries?.edges?.Select(x => x.node)?.ToList();
            var expenseView = new ExpenseWalletView
            {
                BankAccounts = listBankAccounts.data.user.bankAccounts,
                Transactions = listBankAcountEdges.OrderBy(x => x.date).ToList(),
                DebitOrders = debitOrders,
                SalaryInformation = salaryInfoNodes ?? Default.GetSalaryInfo,
                Income = salaryInfo?.data?.user?.income ?? Default.Income,
                FloatBalance = _floatService.GetFloatBalance()
            };
            return expenseView;
        }

        public async Task<TransactionCategoryView> GetTransactionCategoryView()
        {
            var token = _stitchRequestHelper.GetDefaultAuthToken();
            var transactionCategories = await _stitchRequestHelper.GetStitchResponseAsync<StitchResponse>(GraphqlQueries.TransactionCategories, token);
            var expenseCatogories = transactionCategories.data.user.bankAccounts;
            var topSpendingCategories = transactionCategories.data.transactionCategories.ToList();
            var transactionCatagoryView = new TransactionCategoryView
            {
                BankAccounts = expenseCatogories,
                TopSpendingCategories = topSpendingCategories
            };
            return transactionCatagoryView;
        }

        private async Task<List<Node>> GetDebitOrders(string query, AuthenticationToken authenticationToken)
        {
            var debitOrderList = await _stitchRequestHelper.GetStitchResponseAsync<StitchResponse>(query,authenticationToken);
            var debitOrderEdges = debitOrderList.data.user.bankAccounts.Where(x => x.debitOrderPayments != null).SelectMany(x => x.debitOrderPayments.edges);
            return debitOrderEdges.Select(x => x.node).ToList();
        }
      
        public TopUpWalletView GetTopUpWalletView()
        {
            var floatBalance = _floatService.GetFloatBalance();
            var Float = _floatService.GetFloat();
            return new TopUpWalletView
            {
                Float = Float,
                FloatBalance = floatBalance
            };
        }
        private async Task SaveUserToken(AuthenticationToken token)
        {
            var userToken = JsonConvert.SerializeObject(token);
            _inputOutputHelper.Write(SecurityUtilities.UserTokenJsonPath, userToken);

        }
    }
}
