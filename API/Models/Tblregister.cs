using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tblregister
{
    public int Id { get; set; }

    public string? CustName { get; set; }

    public string? Nic { get; set; }

    public string? Email { get; set; }

    public string? CustAddress { get; set; }

    public string? Mobile { get; set; }

    public string? AlternatePhone { get; set; }

    public string? ContactNo { get; set; }

    public string? Otp { get; set; }

    public string? CustPassword { get; set; }

    public DateTime? AddOn { get; set; }

    public string? Photo { get; set; }

    public ulong? CustStatus { get; set; }

    public decimal? AccountBalance { get; set; }

    public string? Hash { get; set; }

    public int? AvatarNo { get; set; }

    public string? Role { get; set; }

    public DateTime? Dob { get; set; }

    public int? AssignedSupervisorId { get; set; }
}
