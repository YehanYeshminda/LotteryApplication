﻿using System;
using System.Collections.Generic;

namespace API.API.Repos.Models;

public partial class Tbldrawhistory
{
    public int Id { get; set; }

    public int? LotteryId { get; set; }

    public DateTime? DrawDate { get; set; }

    public string? Sequence { get; set; }
}
