using System.Globalization;
using System.Security.Cryptography;
using System.Text;


namespace Core.ExpenseWallet
{
    public static class SecurityUtilities
    {
        public static byte[] GenerateHash(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            var sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(bytes);
            return hash;
        }

        public static async Task<VerifiedChallenge> GenerateVerifierChallengePair()
        {
            byte[] randombytes = new byte[32];
            RandomNumberGenerator.Create().GetBytes(randombytes);
            var verified = EncodeBytesToString(randombytes);
            string challengeString = EncodeBytesToString(GenerateHash(verified));
            return new VerifiedChallenge { Verfier = verified, Challenge = challengeString };
        }

        public static string GetStateOrNonce()
        {
            byte[] randombytes = new byte[32];
            RandomNumberGenerator.Create().GetBytes(randombytes);

            return EncodeBytesToString(randombytes);
        }
        private static string EncodeBytesToString(byte[] input)
        {
            return System.Convert.ToBase64String(input).Replace("=", "").Replace('+', '-').Replace('/', '_');
        }
        public static bool IsValidReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) { return false; }
            if (reference.Any(x => !(char.IsDigit(x) || char.IsLetter(x)))) { return false; }
            return true;
        }
        public static bool IsValidAmount(string amount)
        {
            if (string.IsNullOrEmpty(amount)) { return false; }
            if (amount.Any(x => !(char.IsNumber(x) || x == '.'))) { return false; }
            var amt = Convert.ToDouble(amount, CultureInfo.InvariantCulture);
            if(amt < 0.01) { return false; }
            return true;
        }
        public static string StitchSettingsJsonPath = "StitchSession.json";
        public static string UserTokenJsonPath = "UserToken.json";
        public static string ClientTokenJsonPath = "ClientTokens.json";
        public static string FloatsJsonPath = "Floats.json";
        public static string MfaRequiredTopUps = "MfaRequiredTopUps.json";
    }

    public class VerifiedChallenge
    {
        public string Verfier { get; set; }
        public string Challenge { get; set; }
    }



}