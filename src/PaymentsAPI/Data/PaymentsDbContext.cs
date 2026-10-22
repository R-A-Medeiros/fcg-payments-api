using Microsoft.EntityFrameworkCore;
using PaymentsAPI.Domain;

namespace PaymentsAPI.Data;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(e =>
        {
            e.ToTable("payments");
            e.HasKey(p => p.Id);
            e.Property(p => p.Amount).HasColumnType("numeric(10,2)");
            e.Property(p => p.Result).HasConversion<string>().HasMaxLength(20);
            e.Property(p => p.Reason).HasMaxLength(300);
            e.HasIndex(p => p.OrderId).IsUnique();
        });
    }
}
