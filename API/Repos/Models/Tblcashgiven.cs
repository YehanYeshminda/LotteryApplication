using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Repos.Models;

[Table("tblcashgiven")]
public partial class Tblcashgiven
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CashGivenDate { get; set; }

    [Column(TypeName = "int(11)")]
    public int? GivenMoneyId { get; set; }

    [Column(TypeName = "int(11)")]
    public int? UserId { get; set; }

    [StringLength(200)]
    public string? Bank { get; set; }

    [StringLength(200)]
    public string? Branch { get; set; }

    [StringLength(200)]
    public string? AccNo { get; set; }

    [Precision(10, 0)]
    public decimal? Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PayOn { get; set; }
}
