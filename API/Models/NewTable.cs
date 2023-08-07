using System;
using System.Collections.Generic;

namespace API.Models;

public partial class NewTable
{
    public int Id { get; set; }

    public string? LottoNumbers { get; set; }

    public int? UserId { get; set; }

    public string? ReferenceId { get; set; }
}
