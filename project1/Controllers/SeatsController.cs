using Microsoft.AspNetCore.Mvc;
using project1.Repositories;
using project1.Models;

namespace project1.Controllers
{
    [ApiController]
    [Route("api/seats")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatRepository _repository;

        public SeatsController(ISeatRepository repository)
        {
            _repository = repository;
        }

        // -------------------------
        // Seat status
        // -------------------------
        [HttpPost("status")]
        public IActionResult GetStatus([FromBody] SeatStatusRequest request)
        {
            return Ok(_repository.GetSeatStatus(request.ShowId));
        }

        // -------------------------
        // Hold seats
        // -------------------------
        [HttpPost("hold")]
        public IActionResult HoldSeats([FromBody] HoldSeatsRequest request)
        {
            var success = _repository.HoldSeats(request.ShowId, request.SeatCount);

            return success
                ? Ok(new MessageResponse("Seats held successfully"))
                : BadRequest(new MessageResponse("Not enough seats available"));
        }

        // -------------------------
        // Book seats (UPDATED)
        // -------------------------
        [HttpPost("book")]
        public IActionResult BookSeats([FromBody] BookSeatsRequest request)
        {
            var result = _repository.BookSeats(request.ShowId);

            return Ok(result); // returns BookingId, BookedAt, SeatsBooked
        }

        // -------------------------
        // Release expired holds
        // -------------------------
        [HttpPost("release-expired")]
        public IActionResult ReleaseExpired()
        {
            int released = _repository.ReleaseExpiredSeats();
            return Ok(new MessageResponse($"{released} seats released"));
        }
    }
}
