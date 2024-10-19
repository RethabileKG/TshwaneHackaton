using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Team_12.Services;

namespace Team_12.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRVerificationController : ControllerBase
    {
        private readonly IQRVerificationService _qrVerificationService;

        public QRVerificationController(IQRVerificationService qrVerificationService)
        {
            _qrVerificationService = qrVerificationService;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyQRCode([FromBody] string qrContent)
        {
            var isValid = await _qrVerificationService.VerifyAndMarkQRCode(qrContent);

            if (!isValid)
                return BadRequest(new { message = "Invalid or already used QR code" });

            return Ok(new { message = "QR code verified successfully" });
        }
    }
}