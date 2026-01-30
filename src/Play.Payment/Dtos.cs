namespace Play.Payment;

public record ProcessPaymentDto(
    Guid OrderId,
    decimal Amount,
    string PaymentMethod
);

public record PaymentResponseDto(
    Guid PaymentId,
    string Status,
    DateTimeOffset CreatedDate
);
