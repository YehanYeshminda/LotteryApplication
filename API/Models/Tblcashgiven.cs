using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tblcashgiven
{
    public int Id { get; set; }

    public DateTime? CashGivenDate { get; set; }

    public int? GivenMoneyId { get; set; }

    public int? UserId { get; set; }

    public string? Bank { get; set; }

    public string? Branch { get; set; }

    public string? AccNo { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? PayOn { get; set; }
}
