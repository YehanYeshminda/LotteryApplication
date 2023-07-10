namespace API.Repos.Dtos
{
    public class MegaDrawGenDto
    {
        public List<int> TicketNos { get; set; }
        public int RaffleId { get; set; }
        public AuthDto authDto { get; set; }
    }
}
