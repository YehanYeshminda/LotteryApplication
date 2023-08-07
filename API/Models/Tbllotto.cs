using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tbllotto
{
    public int Id { get; set; }

    public string? LottoNumbers { get; set; }

    public int? UserId { get; set; }

    public string? ReferenceId { get; set; }

    public DateTime? AddOn { get; set; }
}
