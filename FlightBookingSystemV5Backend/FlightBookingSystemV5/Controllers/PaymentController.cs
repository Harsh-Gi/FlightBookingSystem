using FlightBookingSystemV5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FlightBookingSystemV5.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Payment(PaymentData paymentData)
        {
            return Ok();
        }
    }
}
