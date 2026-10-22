using PaymentsAPI.Domain;

namespace PaymentsAPI.Services;

public interface IPaymentProcessor
{
    (PaymentResult result, string reason) Process(decimal amount);
}

// Simulates a payment gateway. Approval rule is intentionally simple and configurable so
// both Approved and Rejected paths can be demonstrated:
//   - amount <= 0                -> Rejected (invalid amount)
//   - amount >  ApprovalCeiling  -> Rejected (over limit)
//   - otherwise                  -> Approved
public class PaymentProcessor(IConfiguration configuration) : IPaymentProcessor
{
    private readonly decimal _approvalCeiling =
        decimal.TryParse(configuration["Payments:ApprovalCeiling"], out var v) ? v : 1000m;

    public (PaymentResult result, string reason) Process(decimal amount)
    {
        if (amount <= 0)
            return (PaymentResult.Rejected, "Invalid amount.");

        if (amount > _approvalCeiling)
            return (PaymentResult.Rejected, $"Amount {amount:0.00} exceeds approval ceiling {_approvalCeiling:0.00}.");

        return (PaymentResult.Approved, "Payment approved.");
    }
}
