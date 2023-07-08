using API.Repos.Dtos;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace API.Helpers
{
    public static class PasswordHelpers
    {
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                var enteredHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return enteredHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static HelperAuth DecodeValue(string encodedValue)
        {
            byte[] bytes = Convert.FromBase64String(encodedValue);
            string valueToDecode = Encoding.UTF8.GetString(bytes);

            string[] parts = valueToDecode.Split('!');
            if (parts.Length == 2)
            {
                int userId = int.Parse(parts[0]);
                DateTime dateTime = DateTime.ParseExact(parts[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                return new HelperAuth
                {
                    Date = dateTime,
                    UserId = userId
                };
            }
            else
            {
                return null;
            }
        }
    }
}
