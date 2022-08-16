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

        public WalletService(ITokenBuilder tokenBuilder, IHttpService httpService, IStitchSettings stitchSettings, IInputOutputHelper inputOutputHelper, IFloatService floatService)
        {
            _tokenBuilder = tokenBuilder;
            _httpService = httpService;
            _stitchSettings = stitchSettings;
            _inputOutputHelper = inputOutputHelper;
            _floatService = floatService;
        }
  
        public async Task<ExpenseWalletView> GetExpenseWalletView(string code)
        {
            AuthenticationToken token;
            if (string.IsNullOrEmpty(code)) 
            {
                token = GetDefaultAuthToken();
                if(token == null) { throw new Exception("No code provided to get the token"); }
            }
            else
            {
                token = await _tokenBuilder.GetTokenWithCode(code);
            }
            _inputOutputHelper.Write(SecurityUtilities.CodeJsonFilePath, JsonConvert.SerializeObject(token));
            var listBankAccounts = await GetStitchResponseAsync(GraphqlQueries.ListBankAccountTransactions, token);
            var listBankAcountEdges = listBankAccounts.data.user.bankAccounts.Where(x => x.transactions.edges.Any()).SelectMany(x => x.transactions.edges).Select(x => x.node).ToList();
            var debitOrders = await GetDebitOrders(GraphqlQueries.ListOfDebitOrders, token);
            var salaryInfo = await GetStitchResponseAsync(GraphqlQueries.SalaryInformation, token);
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
            var token = GetDefaultAuthToken();
            var transactionCategories = await GetStitchResponseAsync(GraphqlQueries.TransactionCategories, token);
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
            var debitOrderList = await GetStitchResponseAsync(query,authenticationToken);
            var debitOrderEdges = debitOrderList.data.user.bankAccounts.Where(x => x.debitOrderPayments != null).SelectMany(x => x.debitOrderPayments.edges);
            return debitOrderEdges.Select(x => x.node).ToList();
        }
        private async Task<StitchResponse> GetStitchResponseAsync(string query, AuthenticationToken authenticationToken)
        {
            if(authenticationToken == null) 
            {
                authenticationToken = GetDefaultAuthToken(); 
            }
            var request = new StitchRequest
            {
                AuthenticationToken = authenticationToken,
                Query = query
            };
            var headers = new Dictionary<string, string>()
            {
                {$"{Default.HeaderType.Bearer}",request.AuthenticationToken.Access_Token }
            };
            var response = await _httpService.GetGraphqlResponseAsync<StitchResponse>(_stitchSettings.GraphqlUrl, request.Query, headers);
            return response;
        }

        private AuthenticationToken GetDefaultAuthToken()
        {
            var tokenString = _inputOutputHelper.Read(SecurityUtilities.CodeJsonFilePath);
            var token = JsonConvert.DeserializeObject<AuthenticationToken>(tokenString);
            return token;
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
    }
}
