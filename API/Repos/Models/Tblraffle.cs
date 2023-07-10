namespace API.Repos;

public partial class Tblraffle
{
    public uint Id { get; set; }

    public DateTime? RaffleDate { get; set; }

    public DateTime? StartOn { get; set; }

    public DateTime? EndOn { get; set; }

    public ulong? CustStatus { get; set; }

    public int? TicketNo { get; set; }

    public string? RaffleName { get; set; }
}
