namespace API.Repos.Dtos
{
    public class TblLotteryAdvancedDto
    {
        public int Id { get; set; }
        public string? RaffleNo { get; set; }

        public string? LotteryNo { get; set; }

        public int? UserId { get; set; }

        public DateTime? AddOn { get; set; }

        public decimal? AmountToPay { get; set; }

        public decimal? Paid { get; set; }

        public ulong? LotteryStatus { get; set; }
        public string? LotteryName { get; set; }
        public List<int>? Numbers { get; set; }
    }
}
