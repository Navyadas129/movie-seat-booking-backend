using project1.Models;

namespace project1.Repositories
{
    public interface ISeatRepository
    {
        SeatStatusResponse GetSeatStatus(int showId);

        bool HoldSeats(int showId, int seatCount);

        // 🔴 Changed return type
        BookSeatsResponse BookSeats(int showId);

        int ReleaseExpiredSeats();
    }
}
