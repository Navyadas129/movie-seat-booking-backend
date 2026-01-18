# Movie Seat Booking Backend

This project implements a backend system responsible for managing seat availability for a movie show.
The focus is on correctness, concurrency handling, and reliability under real-world conditions.

---

## Problem Scope

The system manages:
- Seat availability
- Temporary seat holds
- Final seat booking

Out of scope:
- Payments
- Authentication
- UI
- Movie or theatre management 

---

## High-Level Design

Each seat belongs to a movie show and can be in one of the following states:
- AVAILABLE
- HELD (temporarily reserved)
- BOOKED

Seat state is stored in the database and updated using transactional operations.

---

## API Endpoints

### 1. Get Seat Status
**POST** `/api/seats/status`

Returns:
- Available seats
- Held seats
- Booked seats

---

### 2. Hold Seats
**POST** `/api/seats/hold`

Temporarily holds seats for a fixed duration (2 minutes).
If insufficient seats are available, the operation fails.

---

### 3. Book Seats
**POST** `/api/seats/book`

Converts held seats into booked seats.
Booking is allowed only if the hold has not expired.

---

### 4. Release Expired Holds
**POST** `/api/seats/release-expired`

Releases seats whose hold time has expired.
This endpoint is idempotent and safe to retry.

---

## Concurrency Handling

The system prevents overbooking using:
- Database transactions
- Conditional updates based on seat status
- Time-based seat expiry

If multiple users try to book simultaneously:
- Only available seats are held
- Overbooking is prevented at the database level

---

## Failure Scenarios Handling

| Scenario | Handling |
|--------|---------|
| User abandons booking | Seats auto-release after expiry |
| User retries request | Idempotent updates |
| Booking success but no response | Booking state persists |
| System restart | Seat state stored in DB |
| Concurrent bookings | Transactions ensure consistency |

---

## Assumptions

- Each seat is uniquely identified
- Seat expiry is time-based
- Cleanup is triggered via API (can be scheduled in production)
- No user-level authorization is required

---

## Limitations

- No payment integration
- No seat selection by number (count-based booking)
- Manual trigger for expired seat cleanup

---

## Tech Stack

- ASP.NET Core Web API
- ADO.NET
- SQL Server / Oracle-compatible logic
- Swagger for API testing
