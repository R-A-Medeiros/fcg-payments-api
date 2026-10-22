namespace PaymentsAPI.Domain;

public enum PaymentResult
{
    Approved,
    Rejected
}

// A processed payment transaction.
public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Amount { get; set; }
    public PaymentResult Result { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
