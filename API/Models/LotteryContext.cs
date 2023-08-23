using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class LotteryContext : DbContext
{
    public LotteryContext()
    {
    }

    public LotteryContext(DbContextOptions<LotteryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tblbankdetail> Tblbankdetails { get; set; }

    public virtual DbSet<Tblcashgiven> Tblcashgivens { get; set; }

    public virtual DbSet<Tblcompany> Tblcompanies { get; set; }

    public virtual DbSet<Tbldrawhistory> Tbldrawhistories { get; set; }

    public virtual DbSet<Tbllotteryno> Tbllotterynos { get; set; }

    public virtual DbSet<Tbllotterywinner> Tbllotterywinners { get; set; }

    public virtual DbSet<Tbllotto> Tbllottos { get; set; }

    public virtual DbSet<Tbllottoorderhistory> Tbllottoorderhistories { get; set; }

    public virtual DbSet<Tblmoneycredit> Tblmoneycredits { get; set; }

    public virtual DbSet<Tblorderhistory> Tblorderhistories { get; set; }

    public virtual DbSet<Tblpackage> Tblpackages { get; set; }

    public virtual DbSet<Tblpackageorderhistory> Tblpackageorderhistories { get; set; }

    public virtual DbSet<Tblraffle> Tblraffles { get; set; }

    public virtual DbSet<Tblregion> Tblregions { get; set; }

    public virtual DbSet<Tblregister> Tblregisters { get; set; }

    public virtual DbSet<Tblrequestwithdrawal> Tblrequestwithdrawals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tblbankdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblbankdetails");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddOn).HasColumnType("datetime");
            entity.Property(e => e.BenificiaryAccountNo)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.BenificiaryIfscCode)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.BenificiaryName)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Upiid)
                .HasMaxLength(105)
                .HasColumnName("UPIId");
            entity.Property(e => e.UserId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tblcashgiven>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblcashgiven");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AccNo)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Amount)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Bank)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Branch)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CashGivenDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.GivenMoneyId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.PayOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
        });

        modelBuilder.Entity<Tblcompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblcompany");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tbldrawhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tbldrawhistory");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DrawDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.LotteryId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Sequence).HasMaxLength(45);
            entity.Property(e => e.UniqueLotteryId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tbllotteryno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tbllotterynos");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.AmountToPay)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LotteryName).HasMaxLength(45);
            entity.Property(e => e.LotteryNo)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LotteryReferenceId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LotteryStatus)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Paid)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RaffleNo)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
        });

        modelBuilder.Entity<Tbllotterywinner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tbllotterywinners");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.DrawDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.Matches)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.RaffleId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.RaffleUniqueId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.TicketNo)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
        });

        modelBuilder.Entity<Tbllotto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tbllotto");

            entity.HasIndex(e => e.LottoUniqueId, "LottoUniqueId_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.LottoCompanyId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LottoName)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LottoPrice)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LottoUniqueId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.WinnerNo)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tbllottoorderhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tbllottoorderhistory");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.LottoNumbers)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LottoUnqueReferenceId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Price)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.ReferenceId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
        });

        modelBuilder.Entity<Tblmoneycredit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblmoneycredit");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Credit)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.MoneyCreditDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.Paid)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RaffleNo)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.WinNo)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tblorderhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblorderhistory");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.LotteryReferenceId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RaffleId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.RaffleUniqueId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.TicketNo)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
        });

        modelBuilder.Entity<Tblpackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblpackages");

            entity.HasIndex(e => new { e.PackageName, e.PackgeUniqueId }, "packageNameUnique").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.AddOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.PackageName).HasMaxLength(45);
            entity.Property(e => e.PackagePrice).HasPrecision(10);
            entity.Property(e => e.PackgeUniqueId).HasMaxLength(45);
        });

        modelBuilder.Entity<Tblpackageorderhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblpackageorderhistory");

            entity.Property(e => e.Id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.AddOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderStatus).HasColumnType("int(10) unsigned");
            entity.Property(e => e.PackageName).HasMaxLength(45);
            entity.Property(e => e.PackageOrderUniqueId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.PackagePrice).HasMaxLength(45);
            entity.Property(e => e.PackageUniqueId).HasMaxLength(45);
            entity.Property(e => e.UserId).HasColumnType("int(10) unsigned");
        });

        modelBuilder.Entity<Tblraffle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblraffle");

            entity.Property(e => e.Id).HasColumnType("int(11) unsigned");
            entity.Property(e => e.CustStatus)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("bit(1)");
            entity.Property(e => e.DrawCount)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.EndOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.RaffleDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.RaffleName)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("raffleName");
            entity.Property(e => e.RafflePrice)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11) unsigned");
            entity.Property(e => e.StartOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.TicketNo)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UniqueRaffleId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.WinAmount).HasPrecision(10);
        });

        modelBuilder.Entity<Tblregion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblregions");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Code)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Country)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RegionName)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tblregister>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblregister");

            entity.HasIndex(e => e.ContactNo, "ContactNo_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CustName, "CustName_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Email, "Email_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Nic, "NIC_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AccountBalance)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.AddOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.AlternatePhone)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.AvatarNo)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.ContactNo)
                .HasMaxLength(100)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CustAddress)
                .HasMaxLength(400)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CustName)
                .HasMaxLength(300)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CustPassword)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CustStatus)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Dob)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Hash)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Mobile)
                .HasMaxLength(100)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Nic)
                .HasMaxLength(100)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("NIC");
            entity.Property(e => e.Otp)
                .HasMaxLength(100)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("OTP");
            entity.Property(e => e.Photo)
                .HasMaxLength(500)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Role)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Tblrequestwithdrawal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblrequestwithdrawal");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddOn).HasColumnType("datetime");
            entity.Property(e => e.Amount)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.BankId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Latitude)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Longitude)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RequestUniqueId)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Status)
                .HasMaxLength(45)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
