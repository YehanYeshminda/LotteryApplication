using System;
using System.Collections.Generic;

namespace API.Repos.Models;

public partial class Tblorderhistory
{
    public int Id { get; set; }

    public int? RaffleId { get; set; }

    public string? TicketNo { get; set; }

    public int? UserId { get; set; }

    public string? RaffleUniqueId { get; set; }

    public DateTime? AddOn { get; set; }

    public string? LotteryReferenceId { get; set; }
}
