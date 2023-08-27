using FlightBookingSystemV5.Models;
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
        private IPaymentRepository paymentRepository;
        public PaymentController(IPaymentRepository paymentRepository)
        {
            this.paymentRepository = paymentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Payment(PaymentData paymentData)
        {
            bool res = paymentRepository.Payment();
            if(res)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
