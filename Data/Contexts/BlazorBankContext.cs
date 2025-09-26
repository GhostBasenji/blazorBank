using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public partial class BlazorBankContext : DbContext
{
    public BlazorBankContext()
    {
    }

    public BlazorBankContext(DbContextOptions<BlazorBankContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountStatus> AccountStatuses { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientLogin> ClientLogins { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeAction> EmployeeActions { get; set; }

    public virtual DbSet<ExchangeRate> ExchangeRates { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=BankingSystem;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Accounts__349DA58690414907");

            entity.HasIndex(e => e.AccountNumber, "IX_Accounts_AccountNumber");

            entity.HasIndex(e => e.ClientId, "IX_Accounts_ClientID");

            entity.HasIndex(e => e.CurrencyId, "IX_Accounts_CurrencyID");

            entity.HasIndex(e => e.DeletionDate, "IX_Accounts_DeletionDate");

            entity.HasIndex(e => e.StatusId, "IX_Accounts_StatusID");

            entity.HasIndex(e => e.AccountNumber, "UQ__Accounts__BE2ACD6F9A622514").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.AccountNumber).HasMaxLength(30);
            entity.Property(e => e.Balance)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrencyId).HasColumnName("CurrencyID");
            entity.Property(e => e.DeletionDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Client).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accounts__Client__403A8C7D");

            entity.HasOne(d => d.CurrencyNavigation).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accounts__Curren__412EB0B6");

            entity.HasOne(d => d.Status).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accounts__Status__4222D4EF");
        });

        modelBuilder.Entity<AccountStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__AccountS__C8EE2043505EC1A8");

            entity.HasIndex(e => e.StatusName, "UQ__AccountS__05E7698A9A205CF1").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__E67E1A04DD96546F");

            entity.HasIndex(e => e.Email, "IX_Clients_Email");

            entity.HasIndex(e => e.PasswordHash, "IX_Clients_PasswordHash");

            entity.HasIndex(e => e.Username, "UQ_Clients_Username").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Clients__A9D1053445AF7A54").IsUnique();

            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<ClientLogin>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PK__ClientLo__4DDA2838FE43494C");

            entity.HasIndex(e => e.ClientId, "IX_ClientLogins_ClientID");

            entity.HasIndex(e => e.LoginTime, "IX_ClientLogins_LoginTime");

            entity.Property(e => e.LoginId).HasColumnName("LoginID");
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.LoginTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.ClientLogins)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClientLog__Clien__4316F928");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyId).HasName("PK__Currenci__14470B10C6D2D189");

            entity.HasIndex(e => e.CurrencyCode, "UQ__Currenci__408426BF993BF6C5").IsUnique();

            entity.Property(e => e.CurrencyId).HasColumnName("CurrencyID");
            entity.Property(e => e.CurrencyCode).HasMaxLength(3);
            entity.Property(e => e.CurrencyName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1770897F7");

            entity.HasIndex(e => e.Email, "IX_Employees_Email");

            entity.HasIndex(e => e.Email, "UQ__Employee__A9D10534BE912D85").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.HireDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Position).HasMaxLength(50);
        });

        modelBuilder.Entity<EmployeeAction>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__Employee__FFE3F4B929BC6A09");

            entity.HasIndex(e => e.ActionDate, "IX_EmployeeActions_ActionDate");

            entity.HasIndex(e => e.EmployeeId, "IX_EmployeeActions_EmployeeID");

            entity.Property(e => e.ActionId).HasColumnName("ActionID");
            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.RelatedEntityId).HasColumnName("RelatedEntityID");
            entity.Property(e => e.RelatedEntityType).HasMaxLength(50);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeActions)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeA__Emplo__440B1D61");
        });

        modelBuilder.Entity<ExchangeRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exchange__3214EC07F69BC385");

            entity.Property(e => e.Rate).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.BaseCurrency).WithMany(p => p.ExchangeRateBaseCurrencies)
                .HasForeignKey(d => d.BaseCurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExchangeRates_Base");

            entity.HasOne(d => d.TargetCurrency).WithMany(p => p.ExchangeRateTargetCurrencies)
                .HasForeignKey(d => d.TargetCurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExchangeRates_Target");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A4B27783051");

            entity.HasIndex(e => e.AccountId, "IX_Transactions_AccountID");

            entity.HasIndex(e => e.TransactionDate, "IX_Transactions_TransactionDate");

            entity.HasIndex(e => e.TransactionTypeId, "IX_Transactions_TransactionTypeID");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Accou__44FF419A");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Trans__45F365D3");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.TransactionTypeId).HasName("PK__Transact__20266CEB987F6569");

            entity.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
