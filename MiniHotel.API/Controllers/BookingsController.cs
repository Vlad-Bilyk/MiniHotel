using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Enums;
using System.Security.Claims;

namespace MiniHotel.API.Controllers
{
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var bookings = await _bookingService.GetBookingsAsync();
            return Ok(bookings);
        }


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

            if (userId == null)
            {
                return BadRequest("User not found");
            }

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

        [HttpPatch("{id:int}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<BookingDto>> CancelBooking(int id)
        {
            return UpdateStatusAsync(id, BookingStatus.Cancelled);
        }

        [HttpPatch("{id:int}/checkin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<BookingDto>> CheckInBooking(int id)
        {
            return UpdateStatusAsync(id, BookingStatus.CheckedIn);
        }

        [HttpPatch("{id:int}/checkout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<BookingDto>> CheckOutBooking(int id)
        {
            return UpdateStatusAsync(id, BookingStatus.CheckedOut);
        }

        [HttpPatch("{id:int}/confirmed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<BookingDto>> ConfirmedBooking(int id)
        {
            return UpdateStatusAsync(id, BookingStatus.Confirmed);
        }

        [HttpPatch("{id:int}/completed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<BookingDto>> CompletedBooking(int id)
        {
            return UpdateStatusAsync(id, BookingStatus.Completed);
        }

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

        private ActionResult HandleException(Exception ex)
        {
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
