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
        public static string JsonFilePath = "StitchSession.json";
        public static string CodeJsonFilePath = "CodeToken.json";
    }

    public class VerifiedChallenge 
    {
        public string Verfier { get; set; }
        public string Challenge { get; set; }
    }

  

}