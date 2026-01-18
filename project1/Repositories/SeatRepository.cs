using System.Data;
using Microsoft.Data.SqlClient;
using project1.Models;

namespace project1.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly string _connectionString;

        public SeatRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        // Get seat status
        public SeatStatusResponse GetSeatStatus(int showId)
        {
            using var connection = CreateConnection();
            connection.Open();

            var cmd = new SqlCommand(@"
                SELECT
                    ISNULL(SUM(CASE WHEN Status = 'AVAILABLE' THEN 1 ELSE 0 END), 0) AS Available,
                    ISNULL(SUM(CASE WHEN Status = 'HELD' THEN 1 ELSE 0 END), 0) AS Held,
                    ISNULL(SUM(CASE WHEN Status = 'BOOKED' THEN 1 ELSE 0 END), 0) AS Booked
                FROM dbo.Seats
                WHERE ShowId = @ShowId", connection);

            cmd.Parameters.Add("@ShowId", SqlDbType.Int).Value = showId;

            using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!reader.Read()) return new SeatStatusResponse();

            return new SeatStatusResponse
            {
                Available = reader.GetInt32(0),
                Held = reader.GetInt32(1),
                Booked = reader.GetInt32(2)
            };
        }

        // Hold seats temporarily
        public bool HoldSeats(int showId, int seatCount)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var cmd = new SqlCommand(@"
                UPDATE TOP (@Count) dbo.Seats
                SET Status = 'HELD',
                    HoldUntil = DATEADD(MINUTE, 2, GETUTCDATE())
                WHERE ShowId = @ShowId
                  AND Status = 'AVAILABLE'", connection, transaction);

            cmd.Parameters.AddWithValue("@ShowId", showId);
            cmd.Parameters.AddWithValue("@Count", seatCount);

            int updated = cmd.ExecuteNonQuery();
            if (updated < seatCount)
            {
                transaction.Rollback();
                return false;
            }

            transaction.Commit();
            return true;
        }

        // Book seats and return BookingId & time
        public BookSeatsResponse BookSeats(int showId)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var bookingId = Guid.NewGuid().ToString();
            var bookedAt = DateTime.UtcNow;

            var cmd = new SqlCommand(@"
                UPDATE dbo.Seats
                SET Status = 'BOOKED',
                    BookingId = @BookingId,
                    BookingTime = @BookedAt,
                    HoldUntil = NULL
                WHERE ShowId = @ShowId
                  AND Status = 'HELD'
                  AND HoldUntil >= GETUTCDATE();", connection, transaction);

            cmd.Parameters.AddWithValue("@ShowId", showId);
            cmd.Parameters.AddWithValue("@BookingId", bookingId);
            cmd.Parameters.AddWithValue("@BookedAt", bookedAt);

            int seatsBooked = cmd.ExecuteNonQuery();
            transaction.Commit();

            return new BookSeatsResponse
            {
                BookingId = bookingId,
                BookedAt = bookedAt,
                SeatsBooked = seatsBooked
            };
        }

        // Release expired holds
        public int ReleaseExpiredSeats()
        {
            using var connection = CreateConnection();
            connection.Open();

            var cmd = new SqlCommand(@"
                UPDATE dbo.Seats
                SET Status = 'AVAILABLE', HoldUntil = NULL
                WHERE Status = 'HELD'
                  AND HoldUntil < GETUTCDATE()", connection);

            return cmd.ExecuteNonQuery();
        }
    }
}
