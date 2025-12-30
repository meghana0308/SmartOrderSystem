namespace SmartOrder.API.Enums;

public enum PaymentStatus
{
    Pending = 1,    // PayLater + active
    Unpaid = 2,     // PayLater + cancelled
    Paid = 3,       // PayNow + active
    Refunded = 4    // PayNow + cancelled
}

