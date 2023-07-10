namespace API.Repos.Dtos
{
    public class CreateEasyDrawDto
    {
        public DateTime? RaffleDate { get; set; }
        public DateTime? StartOn { get; set; } 
        public DateTime? EndOn { get; set; } 
        public ulong? CustStatus { get; set; }
        public int? TicketNo { get; set; }
        public string? RaffleName { get; set; }
        public AuthDto AuthDto { get; set; }
    }
}
