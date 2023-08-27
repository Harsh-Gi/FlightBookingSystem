using Microsoft.AspNetCore.Mvc;

namespace FlightBookingSystemV5.Models
{
    public class PaymentRepository:IPaymentRepository
    {
        public bool Payment()
        {
            return true;
        }
    }
}
