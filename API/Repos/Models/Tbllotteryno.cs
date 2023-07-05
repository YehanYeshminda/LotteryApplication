using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Repos.Models;

[Table("tbllotterynos")]
[Index("RaffleNo", "LotteryNo", Name = "Index_2", IsUnique = true)]
public partial class Tbllotteryno
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [StringLength(200)]
    public string? RaffleNo { get; set; }

    [StringLength(200)]
    public string? LotteryNo { get; set; }

    [Column(TypeName = "int(11)")]
    public int? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? AddOn { get; set; }

    [Precision(10, 0)]
    public decimal? AmountToPay { get; set; }

    [Precision(10, 0)]
    public decimal? Paid { get; set; }

    [Column(TypeName = "bit(1)")]
    public ulong? LotteryStatus { get; set; }
}
