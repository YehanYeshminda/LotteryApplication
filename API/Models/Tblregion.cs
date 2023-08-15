using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tblregion
{
    public int Id { get; set; }

    public string? RegionName { get; set; }

    public string? Code { get; set; }

    public string? Country { get; set; }
}
