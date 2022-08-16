using Core.ExpenseWallet;
using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseWalletTests
{
    public static class Faker
    {
        public static AuthenticationToken AuthenticationToken()
        {
            var _inputOutputHelper = new IOHelper();
            var tokenString = _inputOutputHelper.Read(SecurityUtilities.CodeJsonFilePath);
            var token = JsonConvert.DeserializeObject<AuthenticationToken>(tokenString);
            return token;
        }
        public static string AuthorizationUrl()
        {
            return "https://secure.stitch.money/connect/authorize?client_id=test-b4da9418-b379-448c-9460-cb0260ec286f&code_challenge=rEgxo4inH9SVu0Ac_mUoMkHkZpEtbastxijNzhvHSpA&code_challenge_method=S256&redirect_uri=https%3A%2F%2Flocalhost%3A8000%2Freturn&scope=openid%20openid%20accounts%20balances%20transactions%20accountholders%20client_imageupload%20client_paymentrequest%20paymentinitiationrequest%20client_paymentauthorizationrequest&response_type=code&nonce=x-ylloGQvhJlFdPBcLWzGSbwqqg54qTnkm3aYs6qFo8&state=60VAe4EvFufCxAtKF-xNmEUQ55PvgXZykhnjQO1gS6g";
        }

        public static string GetCode()
        {
            return "sadfkasdjhgo;asidjgi[o0asfas";
        }

        public static FloatPayment GetValidFloatPayment()
        {
            return new FloatPayment()
            {
                Amount = 100.0,
                Balance = 0,
                CreatedDate = DateTime.UtcNow,
                Currency = "ZAR",
                Id = Guid.NewGuid(),
                Reference = "REFx7RQgn"
            };

        }
        public static FloatPayment GetInvalidFloatPayment()
        {
            return new FloatPayment()
            {
                Amount = -100.0,
                Balance = 500,
                CreatedDate = DateTime.UtcNow,
                Currency = "ZAR",
                Id = Guid.NewGuid(),
                Reference = "REFx7RQgn"
            };
        }

        public static Float GetValidFloat()
        {
            var _float = new Float()
            {
                FloatPayments = new List<FloatPayment>()
                 {
                     new FloatPayment()
                     {
                         Id = Guid.NewGuid(),
                         Reference = "Ref12445",
                         Amount = 100.00,
                         Balance = 100.00,
                         CreatedDate = DateTime.UtcNow,
                         Currency = "ZAR"
                     },
                     new FloatPayment()
                     {
                         Id = Guid.NewGuid(),
                         Reference = "Ref12446",
                         Amount = 200.00,
                         Balance = 300.00,
                         CreatedDate = DateTime.UtcNow,
                         Currency = "ZAR"
                     },
                     new FloatPayment()
                     {
                         Id = Guid.NewGuid(),
                         Reference = "REFBOWK6D",
                         Amount = 150.00,
                         Balance = 450.00,
                         CreatedDate = DateTime.UtcNow,
                         Currency = "ZAR"
                     }
                 }
            };
            return _float;
        }
    }
}