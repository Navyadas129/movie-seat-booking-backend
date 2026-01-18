UPDATE Seats
SET Status = 'AVAILABLE',
    HoldUntil = NULL,
    BookingId = NULL,
    BookingTime = NULL
WHERE ShowId = 101;
