using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Repos.Models;

[Table("tblraffle")]
public partial class Tblraffle
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RaffleDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndOn { get; set; }

    [Column(TypeName = "bit(1)")]
    public ulong? CustStatus { get; set; }
}
