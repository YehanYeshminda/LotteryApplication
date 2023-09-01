using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tbllotterywinner
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? TicketNo { get; set; }

    public string? RaffleUniqueId { get; set; }

    public int? Matches { get; set; }

    public DateTime? DrawDate { get; set; }

    public DateTime? AddOn { get; set; }

    public int? RaffleId { get; set; }

    public int? AssignedSupervisorId { get; set; }
}
