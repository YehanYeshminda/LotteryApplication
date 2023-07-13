using System;
using System.Collections.Generic;

namespace API.API.Repos.Models;

public partial class Tblmoneycredit
{
    public int Id { get; set; }

    public DateTime? MoneyCreditDate { get; set; }

    public string? RaffleNo { get; set; }

    public string? WinNo { get; set; }

    public int? UserId { get; set; }

    public decimal? Credit { get; set; }

    public decimal? Paid { get; set; }
}
