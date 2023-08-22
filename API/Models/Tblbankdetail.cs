using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tblbankdetail
{
    public int Id { get; set; }

    public string? BenificiaryAccountNo { get; set; }

    public string? BenificiaryIfscCode { get; set; }

    public string? BenificiaryName { get; set; }

    public string? UserId { get; set; }
}
