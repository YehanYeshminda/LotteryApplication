using System;
using System.Collections.Generic;

namespace API.API.Repos.Models;

public partial class Tblorderhistory
{
    public int Id { get; set; }

    public int? RaffleId { get; set; }

    public string? TicketNo { get; set; }

    public int? UserId { get; set; }
}
