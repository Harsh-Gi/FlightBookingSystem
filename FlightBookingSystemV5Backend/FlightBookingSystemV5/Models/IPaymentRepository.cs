using Microsoft.AspNetCore.Mvc;

namespace FlightBookingSystemV5.Models
{
    public interface IPaymentRepository
    {
        bool Payment();
    }
}
