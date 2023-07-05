using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Repos.Models;

[Table("tblmoneycredit")]
public partial class Tblmoneycredit
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? MoneyCreditDate { get; set; }

    [StringLength(200)]
    public string? RaffleNo { get; set; }

    [StringLength(200)]
    public string? WinNo { get; set; }

    [Column(TypeName = "int(11)")]
    public int? UserId { get; set; }

    [Precision(10, 0)]
    public decimal? Credit { get; set; }

    [Precision(10, 0)]
    public decimal? Paid { get; set; }
}
