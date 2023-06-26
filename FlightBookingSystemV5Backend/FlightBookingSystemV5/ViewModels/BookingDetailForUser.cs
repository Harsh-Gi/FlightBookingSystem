namespace FlightBookingSystemV5.ViewModels
{
    public class BookingDetailForUser
    {
        public int BookingId { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string SeatNo { get; set; }
        public string StartLoc { get; set; }
        public string EndLoc { get; set; }
        public DateTime StartTime { get; set; }
        public string AirlineName { get;set; }
    }
}
