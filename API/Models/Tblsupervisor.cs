using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tblsupervisor
{
    public int Id { get; set; }

    public int? UnderUserId { get; set; }

    public int? SupervisorId { get; set; }

    public string? CouponNo { get; set; }
}
