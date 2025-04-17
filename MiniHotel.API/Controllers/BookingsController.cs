using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Enums;
using System.Security.Claims;

namespace MiniHotel.API.Controllers
{
    /// <summary>
    /// Controller for managing bookings, including retrieving, creating, and updating booking statuses.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all bookings.
        /// </summary>
        /// <returns>A collection of booking data transfer objects.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var bookings = await _bookingService.GetBookingsAsync();
            return Ok(bookings);
        }

        /// <summary>
        /// Gets all bookings made by the currently authenticated user.
        /// </summary>
        /// <returns>A list of <see cref="UserBookingsDto"/> representing the user's bookings.</returns>
        /// <response code="200">Returns the list of bookings.</response>
        /// <response code="400">If the user ID is not found in the token claims.</response>
        /// <response code="500">If an unexpected server error occurs.</response>
        [HttpGet("user")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserBookingsDto>>> GetUserBookings()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User not found");
            }
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }

        /// <summary>
        /// Retrieves a booking by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the booking.</param>
        /// <returns>A booking data transfer object.</returns>
        /// <response code="200">If the booking is found.</response>
        /// <response code="404">If the booking is not found.</response>
        [HttpGet("{id:int}", Name = "GetBookingById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            try
            {
                var bookingDto = await _bookingService.GetBookingAsync(b => b.BookingId == id);
                if (bookingDto == null)
                {
                    return NotFound("Booking not found.");
                }

                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="createDto">The booking creation data transfer object.</param>
        /// <returns>The newly created booking data transfer object.</returns>
        /// <response code="201">Returns the created booking.</response>
        /// <response code="400">If provided data is invalid.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingCreateDto createDto)
        {
            if (createDto == null)
            {
                return BadRequest("Bad data for booking");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User not found");
            }

            Console.WriteLine(userId);

            try
            {
                var bookingDto = await _bookingService.CreateBookingAsync(createDto, userId);
                return CreatedAtRoute(nameof(GetBookingById), new { id = bookingDto.BookingId }, bookingDto);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingDto>> CreateBookingByAdmin([FromBody] BookingCreateByAdminDto createDto)
        {
            try
            {
                var result = await _bookingService.CreateOfflineBookingAsync(createDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cancels an existing booking.
        /// </summary>
        /// <param name="id">The unique identifier of the booking to cancel.</param>
        /// <returns>The updated booking data transfer object with status set to Cancelled.</returns>
        [HttpPatch("{id:int}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<BookingDto>> CancelBooking(int id)
        {
            return UpdateStatusAsync(id, BookingStatus.Cancelled);
        }

        /// <summary>
        /// Marks a booking as checked in.
        /// </summary>
        /// <param name="id">The unique identifier of the booking to check in.</param>
        /// <returns>The updated booking data transfer object with status set to CheckedIn.</returns>
        [HttpPatch("{id:int}/checkin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<BookingDto>> CheckInBooking(int id)
        {
            return UpdateStatusAsync(id, BookingStatus.CheckedIn);
        }

        /// <summary>
        /// Marks a booking as checked out.
        /// </summary>
        /// <param name="id">The unique identifier of the booking to check out.</param>
        /// <returns>The updated booking data transfer object with status set to CheckedOut.</returns>
        [HttpPatch("{id:int}/checkout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<BookingDto>> CheckOutBooking(int id)
        {
            return UpdateStatusAsync(id, BookingStatus.CheckedOut);
        }

        /// <summary>
        /// Confirms an existing booking.
        /// </summary>
        /// <param name="id">The unique identifier of the booking to confirm.</param>
        /// <returns>The updated booking data transfer object with status set to Confirmed.</returns>
        [HttpPatch("{id:int}/confirmed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<BookingDto>> ConfirmedBooking(int id)
        {
            return UpdateStatusAsync(id, BookingStatus.Confirmed);
        }

        /// <summary>
        /// Updates the status of a booking.
        /// </summary>
        /// <param name="id">The unique identifier of the booking.</param>
        /// <param name="newStatus">The new status to apply to the booking.</param>
        /// <returns>The updated booking data transfer object.</returns>
        private async Task<ActionResult<BookingDto>> UpdateStatusAsync(int id, BookingStatus newStatus)
        {
            try
            {
                var bookingDto = await _bookingService.UpdateBookingStatusAsync(id, newStatus);
                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Handles exceptions thrown during booking operations and returns appropriate HTTP responses.
        /// </summary>
        /// <param name="ex">The exception that was thrown.</param>
        /// <returns>An <see cref="ActionResult"/> representing the HTTP response.</returns>
        private ActionResult HandleException(Exception ex)
        {
            _logger.LogError(ex.Message);
            switch (ex)
            {
                case KeyNotFoundException:
                    return NotFound("Booking not found.");
                case InvalidOperationException:
                    return BadRequest("Invalid operation: " + ex.Message);
                default:
                    _logger.LogError(ex, "Unhandled error.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }
    }
}
