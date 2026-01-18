-- Create Table
CREATE TABLE Seats (
    SeatId INT NOT NULL PRIMARY KEY,
    ShowId INT NOT NULL,
    SeatNumber VARCHAR(10) NULL,
    Status VARCHAR(10) NULL,
    HoldUntil DATETIME NULL,
    BookingId VARCHAR(50) NULL,
    BookingTime DATETIME NULL
);

-- Insert Sample Data
INSERT INTO Seats (SeatId, ShowId, SeatNumber, Status, HoldUntil, BookingId, BookingTime)
VALUES
(1, 101, 'A1', 'AVAILABLE', NULL, NULL, NULL),
(2, 101, 'A2', 'AVAILABLE', NULL, NULL, NULL),
(3, 101, 'A3', 'AVAILABLE', NULL, NULL, NULL);
