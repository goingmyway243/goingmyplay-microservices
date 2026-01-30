using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Play.Payment.Controllers;

[ApiController]
[Route("payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    [HttpPost]
    public ActionResult<PaymentResponseDto> Post(ProcessPaymentDto processPaymentDto)
    {
        var response = new PaymentResponseDto(
            Guid.NewGuid(),
            "Success",
            DateTimeOffset.UtcNow
        );

        return Ok(response);
    }
}
