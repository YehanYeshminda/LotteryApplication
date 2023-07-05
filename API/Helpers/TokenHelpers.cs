using System.Security.Cryptography;
using System.Text;

namespace API.Helpers
{
    public static class TokenHelpers
    {
        public static string GenerateToken()
        {
            const int tokenLength = 20;

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] tokenBytes = new byte[tokenLength];
                rng.GetBytes(tokenBytes);

                string token = Convert.ToBase64String(tokenBytes);
                return RemoveNonAlphaNumericChars(token);
            }
        }

        private static string RemoveNonAlphaNumericChars(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }
}
