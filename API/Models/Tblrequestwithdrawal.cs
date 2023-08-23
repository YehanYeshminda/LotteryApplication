using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tblrequestwithdrawal
{
    public int Id { get; set; }

    public int? BankId { get; set; }

    public int? Amount { get; set; }

    public int? UserId { get; set; }

    public string? RequestUniqueId { get; set; }

    public string? Longitude { get; set; }

    public string? Latitude { get; set; }

    public string? Status { get; set; }

    public DateTime AddOn { get; set; }
}
