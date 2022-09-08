using Core.ExpenseWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Data
{

    public class AccountHolder
    {
        public string gender { get; set; }
        public string fullName { get; set; }
    }

    public class Amount
    {
        public string quantity { get; set; }
        public string currency { get; set; }
    }

    public class AvailableBalance
    {
        public string quantity { get; set; }
        public string currency { get; set; }
    }

    public class BankAccount
    {
        public string name { get; set; }
        public AvailableBalance availableBalance { get; set; }
        public string accountType { get; set; }
        public string accountNumber { get; set; }
        public AccountHolder accountHolder { get; set; }
        public CurrentBalance currentBalance { get; set; }
        public Transactions transactions { get; set; }
        public DebitOrderPayments debitOrderPayments { get; set; }
    }

    public class CurrentBalance
    {
        public string quantity { get; set; }
        public string currency { get; set; }
    }
    public class Status
    {
        public string __typename { get; set; }
    }

    public class UserInitiatePayment
    {
        public PaymentInitiation paymentInitiation { get; set; }
    }
    public class PaymentInitiation
    {
        public Amount amount { get; set; }
        public DateTime date { get; set; }
        public string id { get; set; }
        public Status status { get; set; }
    }
    public class Data
    {
        public User user { get; set; }
        public List<TransactionCategory> transactionCategories { get; set; }
        public ClientPaymentAuthorizationRequestCreate clientPaymentAuthorizationRequestCreate { get; set; }
        public UserInitiatePayment userInitiatePayment { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
    }
    public class DebitOrderPayments
    {
        public List<Edge> edges { get; set; }
    }
    public class Node
    {
        public string id { get; set; }
        public Amount amount { get; set; }
        public string reference { get; set; }
        public RunningBalance runningBalance { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public string frequency { get; set; }
        public DateTime nextSalaryPaymentExpectedDate { get; set; }
        public string nextSalaryPaymentStandardDeviationInDays { get; set; }
        public DateTime previousSalaryPaymentDate { get; set; }
        public SalaryExpected salaryExpected { get; set; }
        public Category category { get; set; }
    }
    public class Category
    {
        public string categoryId { get; set; }
        public string description { get; set; }
        public string probability { get; set; }
    }
    public class Error
    {
        public List<Location> Locations { get; set; }
        public string Message { get; set; }
        public List<string> Path { get; set; }
        public Extensions Extensions { get; set; }
    }
    public class Location
    {
        public int Column { get; set; }
        public int Line { get; set; }
    }

    public class Extensions
    {
        public string userInteractionUrl { get; set; }
        public string id { get; set; }
        public string code { get; set; }
    }

    public class StitchResponse
    {
        public Data data { get; set; }
        public List<Error> Errors { get; set; }
        public bool HasErrors => Errors != null && Errors.Any();
    }
    public class ClientPaymentAuthorizationRequestCreate
    {
        public string authorizationRequestUrl { get; set; }
    }

    public class RunningBalance
    {
        public string quantity { get; set; }
        public string currency { get; set; }
    }

    public class Transactions
    {
        public List<Edge> edges { get; set; }
    }

    public class User
    {
        public string __typename { get; set; }
        public List<BankAccount> bankAccounts { get; set; }
        public Salaries salaries { get; set; }
        public Income income { get; set; }
    }

    public class Salaries
    {
        public List<Edge> edges { get; set; }
    }

    public class SalaryExpected
    {
        public string quantity { get; set; }
        public string currency { get; set; }
    }

    public class TotalIncome
    {
        public string quantity { get; set; }
        public string currency { get; set; }
    }
    public class Income
    {
        public TotalIncome totalIncome { get; set; }
    }
}
