using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tbllotto
{
    public int Id { get; set; }

    public string? LottoName { get; set; }

    public string? LottoUniqueId { get; set; }

    public decimal? LottoPrice { get; set; }

    public string? LottoCompanyId { get; set; }
}
