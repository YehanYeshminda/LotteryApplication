using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tblpackageorderhistory
{
    public uint Id { get; set; }

    public string PackageName { get; set; } = null!;

    public string PackageUniqueId { get; set; } = null!;

    public string PackagePrice { get; set; } = null!;

    public uint UserId { get; set; }

    public string? PackageOrderUniqueId { get; set; }

    public DateTime? AddOn { get; set; }
}
