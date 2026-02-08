using Microsoft.EntityFrameworkCore;
using AutoLoan.Shared.Entities;

namespace AutoLoan.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<FinancialInfo> FinancialInfos => Set<FinancialInfo>();
    public DbSet<ApplicationNote> ApplicationNotes => Set<ApplicationNote>();
    public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<JwtDenylist> JwtDenylists => Set<JwtDenylist>();
    public DbSet<SecurityAuditLog> SecurityAuditLogs => Set<SecurityAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.ConfirmationToken).IsUnique();
            entity.HasIndex(e => e.ResetPasswordToken).IsUnique();
            entity.HasIndex(e => e.UnlockToken).IsUnique();
            entity.HasIndex(e => e.Jti);
        });

        // Application
        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("applications");
            entity.HasIndex(e => e.ApplicationNumber).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.UserId, e.Status });
            entity.HasOne(e => e.User).WithMany(u => u.Applications).HasForeignKey(e => e.UserId);
            entity.Property(e => e.LoanAmount).HasPrecision(10, 2);
            entity.Property(e => e.DownPayment).HasPrecision(10, 2);
            entity.Property(e => e.InterestRate).HasPrecision(5, 2);
            entity.Property(e => e.MonthlyPayment).HasPrecision(10, 2);
        });

        // Vehicle
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("vehicles");
            entity.HasIndex(e => e.ApplicationId).IsUnique();
            entity.HasIndex(e => e.Vin).IsUnique().HasFilter("\"Vin\" IS NOT NULL");
            entity.HasOne(e => e.Application).WithOne(a => a.Vehicle).HasForeignKey<Vehicle>(e => e.ApplicationId);
            entity.Property(e => e.EstimatedValue).HasPrecision(10, 2);
        });

        // Document
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("documents");
            entity.HasIndex(e => e.Status);
            entity.HasOne(e => e.Application).WithMany(a => a.Documents).HasForeignKey(e => e.ApplicationId);
            entity.HasOne(e => e.VerifiedBy).WithMany(u => u.VerifiedDocuments).HasForeignKey(e => e.VerifiedById);
        });

        // Address
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("addresses");
            entity.HasIndex(e => new { e.ApplicationId, e.AddressType });
            entity.HasOne(e => e.Application).WithMany(a => a.Addresses).HasForeignKey(e => e.ApplicationId);
        });

        // FinancialInfo
        modelBuilder.Entity<FinancialInfo>(entity =>
        {
            entity.ToTable("financial_infos");
            entity.HasIndex(e => new { e.ApplicationId, e.IncomeType });
            entity.HasOne(e => e.Application).WithMany(a => a.FinancialInfos).HasForeignKey(e => e.ApplicationId);
            entity.Property(e => e.AnnualIncome).HasPrecision(12, 2);
            entity.Property(e => e.MonthlyIncome).HasPrecision(10, 2);
            entity.Property(e => e.OtherIncome).HasPrecision(10, 2);
            entity.Property(e => e.MonthlyExpenses).HasPrecision(10, 2);
        });

        // ApplicationNote
        modelBuilder.Entity<ApplicationNote>(entity =>
        {
            entity.ToTable("application_notes");
            entity.HasOne(e => e.Application).WithMany(a => a.ApplicationNotes).HasForeignKey(e => e.ApplicationId);
            entity.HasOne(e => e.User).WithMany(u => u.ApplicationNotes).HasForeignKey(e => e.UserId);
        });

        // StatusHistory
        modelBuilder.Entity<StatusHistory>(entity =>
        {
            entity.ToTable("status_histories");
            entity.HasOne(e => e.Application).WithMany(a => a.StatusHistories).HasForeignKey(e => e.ApplicationId);
            entity.HasOne(e => e.User).WithMany(u => u.StatusHistories).HasForeignKey(e => e.UserId);
        });

        // ApiKey
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.ToTable("api_keys");
            entity.HasOne(e => e.User).WithMany(u => u.ApiKeys).HasForeignKey(e => e.UserId);
        });

        // JwtDenylist
        modelBuilder.Entity<JwtDenylist>(entity =>
        {
            entity.ToTable("jwt_denylists");
            entity.HasIndex(e => e.Jti);
        });

        // SecurityAuditLog
        modelBuilder.Entity<SecurityAuditLog>(entity =>
        {
            entity.ToTable("security_audit_logs");
        });
    }
}
