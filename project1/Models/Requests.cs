namespace project1.Models
{
    public record SeatStatusRequest(int ShowId);
    public record HoldSeatsRequest(int ShowId, int SeatCount);
    public record BookSeatsRequest(int ShowId);
}
