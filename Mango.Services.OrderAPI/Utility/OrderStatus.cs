namespace Mango.Services.OrderAPI.Utility
{
    public enum OrderStatus
    {
        Pending, // Order has been created but not yet processed
        PaymentLater, // Payment is expected later, not immediately processed
        PaymentReceived, // Payment has been received for the order
        PaymentFailed, // Payment attempt failed
        PaymentMismatch, // Payment received but does not match order amount
        Shipped, // Order has been shipped to the customer
        Delivered, // Order has been delivered to the customer
        Cancelled, // Order has been cancelled by the customer or system
        Refunded // Order has been refunded to the customer
    }
}
