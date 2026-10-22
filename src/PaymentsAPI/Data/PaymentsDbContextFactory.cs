using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PaymentsAPI.Data;

// Used only by the EF Core CLI tools at design time (migrations/scripts). Does NOT connect.
public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=fcg_payments;Username=postgres;Password=postgres")
            .Options;

        return new PaymentsDbContext(options);
    }
}
