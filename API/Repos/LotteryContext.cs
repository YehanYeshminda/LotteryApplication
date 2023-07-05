using System;
using System.Collections.Generic;
using API.Repos.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repos;

public partial class LotteryContext : DbContext
{
    public LotteryContext()
    {
    }

    public LotteryContext(DbContextOptions<LotteryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tblcashgiven> Tblcashgivens { get; set; }

    public virtual DbSet<Tbllotteryno> Tbllotterynos { get; set; }

    public virtual DbSet<Tblmoneycredit> Tblmoneycredits { get; set; }

    public virtual DbSet<Tblraffle> Tblraffles { get; set; }

    public virtual DbSet<Tblregister> Tblregisters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Server=127.0.0.1;port=3306;database=lottery;uid=root;pwd=123;Convert Zero Datetime=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tblcashgiven>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.AccNo).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Amount).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Bank).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Branch).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CashGivenDate).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.GivenMoneyId).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.PayOn).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId).HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tbllotteryno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.AddOn).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.AmountToPay).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LotteryNo).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LotteryStatus).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Paid).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RaffleNo).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId).HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tblmoneycredit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Credit).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.MoneyCreditDate).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Paid).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RaffleNo).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.WinNo).HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tblraffle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CustStatus).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.EndOn).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RaffleDate).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.StartOn).HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tblregister>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.AccountBalance).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.AddOn).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.AlternatePhone).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.ContactNo).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CustAddress).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CustName).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CustPassword).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CustStatus).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Email).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Hash).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Mobile).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Nic).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Otp).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Photo).HasDefaultValueSql("'NULL'");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
