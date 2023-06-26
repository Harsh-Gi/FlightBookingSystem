using FlightBookingSystemV5.Auth;
using FlightBookingSystemV5.Models;
using FlightBookingSystemV5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.OpenApi.Any;
using Org.BouncyCastle.Crypto.Macs;
using System.Security.Claims;
using User.Management.Service.Model;
using User.Management.Service.Services;

namespace FlightBookingSystemV5.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]

    public class BookingController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext context;
        private readonly IEmailService _emailService;
        public BookingController(UserManager<IdentityUser> userManager, ApplicationDbContext context, IEmailService emailService)
        {
            this._userManager = userManager;
            this.context = context;
            _emailService = emailService;
        }
        [HttpPost]
        //[Route("Booking")]
        public async Task<IActionResult> Booking(List<BookingData> bookingDataList)
        {
            //var user = await _userManager.GetUserAsync(HttpContext.User);
            //var userId = user.Id;
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userData = _userManager.FindByIdAsync(userId);
            //string userId = "7900fcc6-4328-4f20-a0da-46de7bfbd971";

            //if (userId != null)
            //{
            var resultList = new List<BookingDetailResult>();
            string messageStr= "Your booking is succesfully done!\n";
            foreach(var bookingData in bookingDataList)
            {
                int sJId = bookingData.JourneyId;
                JourneyDetail journeyDetail = context.JourneyDetails.FirstOrDefault(jd => jd.JourneyId == sJId);
                BookingDetail bookingDetail = new BookingDetail();
                bookingDetail.UserName = bookingData.UserName;
                bookingDetail.Age = bookingData.Age;
                bookingDetail.Gender = bookingData.Gender;
                bookingDetail.UserId = bookingData.UserId;
                bookingDetail.JourneyId = sJId;

                if (bookingData.TicketType == "Economy" && journeyDetail.EClassAvailableSeats > 0)
                {
                    //bookingDetail.SeatNo = "E" + (journeyDetail.EClassCapacity - journeyDetail.EClassAvailableSeats + 1);
                    if(journeyDetail.EClassCurrSeatNo < journeyDetail.EClassCapacity)
                    {
                        bookingDetail.SeatNo = "E" + (journeyDetail.EClassCurrSeatNo + 1);
                        journeyDetail.EClassCurrSeatNo += 1;
                    }
                    else
                    {
                        if (journeyDetail.ECancelledSeatNos.Length > 0)
                        {
                            string[] cancelledSeatNosArray = journeyDetail.ECancelledSeatNos.Split(' ');
                            bookingDetail.SeatNo = "E" + cancelledSeatNosArray[0];
                            string cancelledSeatNosStr = "";
                            int i;
                            for (i=1; i<cancelledSeatNosArray.Length; i++)
                            {
                                cancelledSeatNosStr = cancelledSeatNosStr + cancelledSeatNosArray[i] + " ";
                            }
                            journeyDetail.ECancelledSeatNos = cancelledSeatNosStr;
                        }
                    }
                    journeyDetail.EClassAvailableSeats -= 1;
                }
                else
                {
                    //bookingDetail.SeatNo = "B" + (journeyDetail.BClassCapacity - journeyDetail.BClassAvailableSeats + 1);

                    //bookingDetail.SeatNo = "B" + (journeyDetail.BClassCurrSeatNo + 1);
                    //journeyDetail.BClassAvailableSeats -= 1;
                    //journeyDetail.BClassCurrSeatNo += 1;

                    if (journeyDetail.BClassCurrSeatNo < journeyDetail.BClassCapacity)
                    {
                        bookingDetail.SeatNo = "B" + (journeyDetail.BClassCurrSeatNo + 1);
                        journeyDetail.BClassCurrSeatNo += 1;
                    }
                    else
                    {
                        if (journeyDetail.BCancelledSeatNos.Length > 0)
                        {
                            string[] cancelledSeatNosArray = journeyDetail.BCancelledSeatNos.Split(' ');
                            bookingDetail.SeatNo = "B" + cancelledSeatNosArray[0];
                            string cancelledSeatNosStr = "";
                            int i;
                            for (i = 1; i < cancelledSeatNosArray.Length; i++)
                            {
                                cancelledSeatNosStr = cancelledSeatNosStr + cancelledSeatNosArray[i] + " ";
                            }
                            journeyDetail.BCancelledSeatNos = cancelledSeatNosStr;
                        }
                    }
                    journeyDetail.BClassAvailableSeats -= 1;
                }

                context.BookingDetails.Add(bookingDetail);
                context.SaveChanges();
                if (bookingDetail.BookingId != 0)
                {
                    messageStr = messageStr +
                        "\nBooking ID: " + bookingDetail.BookingId +
                        "\nPassenger name: " + bookingDetail.UserName +
                        "\nSeat no. " + bookingDetail.SeatNo +
                        "\nJourney from: " + journeyDetail.StartLoc +
                        "\nJourney to: " + journeyDetail.EndLoc +
                        "\nJourney date: " + journeyDetail.StartTime +
                        "\nAirline name: " + journeyDetail.AirlineName + "\n";

                    resultList.Add(new BookingDetailResult
                    {
                        BookingId = bookingDetail.BookingId.ToString(),
                        PassengerName = bookingDetail.UserName,
                        Age = bookingDetail.Age.ToString(),
                        Gender=bookingDetail.Gender,
                        SeatNo = bookingDetail.SeatNo,
                        Airline = journeyDetail.AirlineName
                    });
                    //return Ok(new
                    //{
                    //    Status = "Success",
                    //    Message = "Booking Successfull",
                    //    BookingId = bookingDetail.BookingId.ToString(),
                    //    SeatNo = bookingDetail.SeatNo,
                    //    Airline = journeyDetail.AirlineName
                    //});
                }
            }
            if(resultList.Count > 0)
            {
                var message = new Message(new string[] { "harshdod.itse@gmail.com" }, "Booking confirmation", messageStr);
                _emailService.SendEmail(message);
                return Ok(resultList);
            }
            //}
            //else
            //{
            //    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "User not signed in", Message = "Please Signin!" });
            //}
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Unable to book the ticket", Message = "Please try again!" });
        }

        [HttpPost]
        [Route("Bookings")]
        public async Task<ActionResult<IEnumerable<BookingDetail>>> Bookings(UserIdModel userIdModel)
        {
            List<BookingDetail> bookingDetails = context.BookingDetails.Where(bd => bd.UserId == userIdModel.UserId).ToList();
            List<BookingDetailForUser> bookingDetailForUserList = new List<BookingDetailForUser>();
            foreach(BookingDetail bookingDetail in bookingDetails)
            {
                int sJId = bookingDetail.JourneyId;
                JourneyDetail journeyDetail = context.JourneyDetails.FirstOrDefault(jd => jd.JourneyId == sJId);
                bookingDetailForUserList.Add(new BookingDetailForUser()
                {
                    BookingId= bookingDetail.BookingId,
                    UserName=bookingDetail.UserName,
                    Age=bookingDetail.Age,
                    Gender=bookingDetail.Gender,
                    SeatNo = bookingDetail.SeatNo,
                    StartLoc=journeyDetail.StartLoc,
                    EndLoc=journeyDetail.EndLoc,
                    StartTime=journeyDetail.StartTime,
                    AirlineName=journeyDetail.AirlineName
                });
            }
            if (bookingDetailForUserList.Count > 0)
            {
                return Ok(bookingDetailForUserList);
            }
            return NotFound("You Have Not Done Any Bookings Yet!");
        }

        //[Route("CancelBooking")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var bookingDetail = await context.BookingDetails.FindAsync(id);
            if (bookingDetail == null)
            {
                return BadRequest();
            }
            int sJId = bookingDetail.JourneyId;
            JourneyDetail journeyDetail = context.JourneyDetails.FirstOrDefault(jd => jd.JourneyId == sJId);
            string seatNo = bookingDetail.SeatNo;
            if (seatNo[0] == 'E')
            {
                journeyDetail.EClassAvailableSeats = journeyDetail.EClassAvailableSeats + 1;
                journeyDetail.ECancelledSeatNos=journeyDetail.ECancelledSeatNos+seatNo.Substring(1)+" ";
            }
            else
            {
                journeyDetail.BClassAvailableSeats = journeyDetail.BClassAvailableSeats + 1;
                journeyDetail.BCancelledSeatNos = journeyDetail.BCancelledSeatNos + seatNo.Substring(1) + " ";
            }
            context.BookingDetails.Remove(bookingDetail);
            await context.SaveChangesAsync();
            string messageStr = "Your booking is cancelled succesfully for:\n"+
                        "\nBooking ID: " + bookingDetail.BookingId +
                        "\nPassenger name: " + bookingDetail.UserName +
                        "\nSeat no. " + bookingDetail.SeatNo +
                        "\nJourney from: " + journeyDetail.StartLoc +
                        "\nJourney to: " + journeyDetail.EndLoc +
                        "\nJourney date: " + journeyDetail.StartTime +
                        "\nAirline name: " + journeyDetail.AirlineName + "\n";
            var message = new Message(new string[] { "harshdod.itse@gmail.com" }, "Booking cancellation", messageStr);
            _emailService.SendEmail(message);
            return Ok();
        }
    }
}
