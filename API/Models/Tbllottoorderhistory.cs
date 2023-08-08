using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tbllottoorderhistory
{
    public int Id { get; set; }

    public string? LottoNumbers { get; set; }

    public int? UserId { get; set; }

    public string? ReferenceId { get; set; }

    public DateTime? AddOn { get; set; }

    public string? Price { get; set; }

    public string? LottoUnqueReferenceId { get; set; }
}
