namespace FCG.Contracts.Events;

// Contracts are duplicated per service by design (each service is an independent repo).
// MassTransit matches messages by the full type name (namespace + type), so these
// records must keep the same namespace and shape across all services.

public enum PaymentStatus
{
    Approved,
    Rejected
}

// Consumed by PaymentsAPI (published by CatalogAPI).
public record OrderPlacedEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public Guid GameId { get; init; }
    public string GameTitle { get; init; } = string.Empty;
    public decimal Price { get; init; }
}

// Published by PaymentsAPI (consumed by CatalogAPI and NotificationsAPI).
public record PaymentProcessedEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public Guid GameId { get; init; }
    public string GameTitle { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public PaymentStatus Status { get; init; }
    public DateTime ProcessedAt { get; init; }
}
