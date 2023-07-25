namespace API.Repos.Dtos
{
    public class GetHistoryDto
    {
        public int? RaffleId { get; set; }
        public string TicketNumber { get; set; }
        public string UniqueRaffleId { get; set; }
        public DateTime? OrderedOn { get; set; }
        public string ReferenceId { get; set; }
    }
}
