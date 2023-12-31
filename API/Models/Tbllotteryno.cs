﻿using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tbllotteryno
{
    public int Id { get; set; }

    public string? RaffleNo { get; set; }

    public string? LotteryNo { get; set; }

    public int? UserId { get; set; }

    public DateTime? AddOn { get; set; }

    public decimal? AmountToPay { get; set; }

    public decimal? Paid { get; set; }

    public int? LotteryStatus { get; set; }

    public string? LotteryReferenceId { get; set; }

    public string LotteryName { get; set; } = null!;
}
