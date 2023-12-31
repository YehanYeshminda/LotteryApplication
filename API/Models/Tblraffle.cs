﻿using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tblraffle
{
    public uint Id { get; set; }

    public DateTime? RaffleDate { get; set; }

    public DateTime? StartOn { get; set; }

    public DateTime? EndOn { get; set; }

    public ulong? CustStatus { get; set; }

    public string? TicketNo { get; set; }

    public string? RaffleName { get; set; }

    public uint? RafflePrice { get; set; }

    public int? DrawCount { get; set; }

    public string? UniqueRaffleId { get; set; }

    public decimal WinAmount { get; set; }
}
