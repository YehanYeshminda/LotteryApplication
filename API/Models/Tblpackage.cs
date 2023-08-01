using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tblpackage
{
    public uint Id { get; set; }

    public string PackageName { get; set; } = null!;

    public decimal PackagePrice { get; set; }

    public string PackgeUniqueId { get; set; } = null!;
}
