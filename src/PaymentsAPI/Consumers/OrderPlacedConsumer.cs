using FCG.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentsAPI.Data;
using PaymentsAPI.Domain;
using PaymentsAPI.Services;

namespace PaymentsAPI.Consumers;

// Consumes OrderPlacedEvent, processes (simulates) the payment, persists the transaction,
// and publishes PaymentProcessedEvent with the result.
public class OrderPlacedConsumer(
    PaymentsDbContext db,
    IPaymentProcessor processor,
    ILogger<OrderPlacedConsumer> logger) : IConsumer<OrderPlacedEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        logger.LogInformation("OrderPlacedEvent received for order {OrderId} (amount {Amount:0.00}).",
            msg.OrderId, msg.Price);

        // Idempotency: if this order was already processed, republish the stored result.
        var existing = await db.Payments.FirstOrDefaultAsync(p => p.OrderId == msg.OrderId, ct);
        PaymentResult result;

        if (existing is not null)
        {
            logger.LogInformation("Order {OrderId} already processed as {Result}. Re-publishing result.",
                msg.OrderId, existing.Result);
            result = existing.Result;
        }
        else
        {
            var (processed, reason) = processor.Process(msg.Price);
            result = processed;

            db.Payments.Add(new Payment
            {
                OrderId = msg.OrderId,
                UserId = msg.UserId,
                GameId = msg.GameId,
                Amount = msg.Price,
                Result = result,
                Reason = reason
            });
            await db.SaveChangesAsync(ct);

            logger.LogInformation("Order {OrderId} processed: {Result} ({Reason}).", msg.OrderId, result, reason);
        }

        await context.Publish(new PaymentProcessedEvent
        {
            OrderId = msg.OrderId,
            UserId = msg.UserId,
            GameId = msg.GameId,
            GameTitle = msg.GameTitle,
            Price = msg.Price,
            Status = result == PaymentResult.Approved ? PaymentStatus.Approved : PaymentStatus.Rejected,
            ProcessedAt = DateTime.UtcNow
        }, ct);

        logger.LogInformation("PaymentProcessedEvent published for order {OrderId}.", msg.OrderId);
    }
}
