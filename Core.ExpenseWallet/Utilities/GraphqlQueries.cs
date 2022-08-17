using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExpenseWallet.Utilities
{
    public static class GraphqlQueries
    {
        public static string ListBankAccountTransactions = @"query ListBankAccountTransactions {

                                  user {
                                    __typename
                                    bankAccounts {
                                      name
                                      availableBalance
                                      accountType
                                      accountNumber
                                      accountHolder {
                                        ... on Individual {
                                          gender
                                          fullName
                                        }
                                      }
                                      currentBalance
                                      transactions {
                                        edges {
                                          node {
                                            id
                                            amount
                                            reference
                                            runningBalance
                                            description
                                            date
                                          }
                                        }
                                      }
                                    }
                                  }

                               }";

        public static string ListBankAccountTypesWithBalances =
         @"{
             user {
               __typename
               bankAccounts {
                 name
                 availableBalance
                 accountNumber
                 currentBalance
                 accountType
               }
             }
           }
        ";

        public static string ListOfDebitOrders =
         @"
           {
             user {
               __typename
               bankAccounts {
                 debitOrderPayments {
                   edges {
                     node {
                       id
                       amount
                       reference
                     }
                   }
                 }
               }
             }
           }

        ";

        public static string SalaryInformation =
        @"
           {
             user {
               __typename
               salaries {
                 edges {
                   node {
                     frequency
                     nextSalaryPaymentExpectedDate
                     nextSalaryPaymentStandardDeviationInDays
                     previousSalaryPaymentDate
                     salaryExpected
                   }
                 }
               }
               income {
                 totalIncome
               }
             }
           }

        ";


        public static string TransactionCategories =
        @"
           query GetTransactionCategories {
             transactionCategories(categorySet: consumer) {
               description
               id
             }
             user {
               id
               bankAccounts {
                 id
                 name
                 transactions {
                   edges {
                     node {
                       category(categorySet: consumer) {
                         categoryId
                         description
                         probability
                       }
                       amount
                       date
                     }
                   }
                 }
               }
             }
          }
        ";

        public static string CreateAccountLinkingRequest =
        @"
           mutation CreateAccountLinkingRequest {
             clientPaymentAuthorizationRequestCreate(input: {
               beneficiary: {
                 bankAccount: {
                   name: ""Sample Account"", 
                   bankId: absa, 
                   accountNumber: ""1234567890"", 
                   accountType: current, 
                   beneficiaryType: private, 
                   reference: ""TestBeneficiary""
                 }
             }, 
              payer:
              {
                  email: ""sampleuser@example.com"",       
                  name: ""Sample User"", 
                  reference: ""TestPayer"",
                  phoneNumber: ""27821234567""
               }
           })
           {
             authorizationRequestUrl
           }
         }

        ";

        public static string UserInitiatePayment =
        @"
           mutation UserInitiatePayment(
               $amount: MoneyInput!,
               $payerReference: String!,
               $externalReference: String) {  
             userInitiatePayment(input: {
                 amount: $amount,
                 payerReference: $payerReference,
                 externalReference: $externalReference
               }) {
               paymentInitiation {
                 amount
                 date
                 id
                 status {
                   __typename
                 }
               }
             }
           }
        ";

    }
}
