using System.Text;
using API.Repos;

namespace API.Helpers
{
    public class Generators
    {
        private static readonly Random random = new Random();
        private readonly LotteryContext _context;
        private const string numericChars = "0123456789";
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public Generators(LotteryContext context)
        {
            _context = context;
        }

        public string GenerateRandomString(int length)
        {
            string raffleId;
            do
            {
                raffleId = GenerateRandomId(length);
            } while (!IsUniqueRaffleId(raffleId));

            return raffleId;
        }

        private string GenerateRandomId(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            sb.Append(chars[random.Next(52, chars.Length)]);
            for (int i = 1; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }

        private bool IsUniqueRaffleId(string raffleId)
        {
            return !_context.Tblraffles.Any(r => r.UniqueRaffleId == raffleId);
        }

        public string GenerateRandomNumericStringForRaffle(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(numericChars[random.Next(numericChars.Length)]);
            }
            return sb.ToString();
        }

        public bool IsUniqueRaffleNoAndId(string raffleNo)
        {
            return !_context.Tbllotterynos.Any(l => l.LotteryNo == raffleNo)
                && !_context.Tblraffles.Any(r => r.UniqueRaffleId == raffleNo);
        }

        public string GenerateRandomNumericStringForLottery(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(numericChars[random.Next(numericChars.Length)]);
            }
            return sb.ToString();
        }

        public bool IsUniqueLotteryNoId(string raffleNo)
        {
            return !_context.Tbllotterynos.Any(l => l.LotteryNo == raffleNo)
                && !_context.Tblraffles.Any(r => r.UniqueRaffleId == raffleNo);
        }
    }
}
