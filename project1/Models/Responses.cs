namespace project1.Models
{
    public record SeatStatusResponse
    {
        public int Available { get; init; }
        public int Held { get; init; }
        public int Booked { get; init; }
    }

    public record MessageResponse(string Message);

    public record BookSeatsResponse
    {
        public string BookingId { get; init; }
        public DateTime BookedAt { get; init; }
        public int SeatsBooked { get; init; }
    }
}
