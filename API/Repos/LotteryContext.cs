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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<Tbllotteryno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tbllotterynos");

            entity.HasIndex(e => new { e.RaffleNo, e.LotteryNo }, "Index_2").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.AmountToPay)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LotteryNo)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.LotteryStatus)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("bit(1)");
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

        modelBuilder.Entity<Tblraffle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblraffle");

            entity.Property(e => e.Id).HasColumnType("int(11) unsigned");
            entity.Property(e => e.CustStatus)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("bit(1)");
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
            entity.Property(e => e.StartOn)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.TicketNo)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
        });

        modelBuilder.Entity<Tblregister>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblregister");

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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
