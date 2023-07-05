using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Repos.Models;

[Table("tblregister")]
public partial class Tblregister
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [StringLength(300)]
    public string? CustName { get; set; }

    [Column("NIC")]
    [StringLength(100)]
    public string? Nic { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(400)]
    public string? CustAddress { get; set; }

    [StringLength(100)]
    public string? Mobile { get; set; }

    [StringLength(200)]
    public string? AlternatePhone { get; set; }

    [StringLength(100)]
    public string? ContactNo { get; set; }

    [Column("OTP")]
    [StringLength(100)]
    public string? Otp { get; set; }

    [StringLength(200)]
    public string? CustPassword { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? AddOn { get; set; }

    [StringLength(500)]
    public string? Photo { get; set; }

    [Column(TypeName = "bit(1)")]
    public ulong? CustStatus { get; set; }

    [Precision(10, 0)]
    public decimal? AccountBalance { get; set; }

    [StringLength(200)]
    public string? Hash { get; set; }
}
