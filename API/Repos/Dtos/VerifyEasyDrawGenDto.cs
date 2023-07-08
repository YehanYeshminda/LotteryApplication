namespace API.Repos.Dtos
{
    public class VerifyEasyDrawGenDto
    {
        public List<int> TicketNos { get; set; }
        public int RaffleId { get; set; }
        public AuthDto authDto { get; set; }
    }
}
