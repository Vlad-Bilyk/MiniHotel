using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.Common;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Exceptions;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Constants;
using MiniHotel.Domain.Enums;
using System.Security.Claims;

namespace MiniHotel.API.Controllers
{
    /// <summary>
    /// Controller for managing bookings, including retrieving, creating, and updating booking statuses.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Retrieves a paginated list of bookings for administrative view.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of records per page (default is 10).</param>
        /// <param name="search">Optional search term to filter bookings by customer name, room number, or room type.</param>
        /// <param name="status">Booking status for filtering (optional).</param>
        /// <returns>A paginated result containing booking data transfer objects.</returns>
        /// <response code="200">Returns the paginated list of bookings.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<ActionResult<PagedResult<BookingDto>>> GetBookings([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] BookingStatus? status = null)
        {
            var result = await _bookingService.GetBookingsAsync(pageNumber, pageSize, search, status);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of bookings specific to the currently authenticated user.
        /// </summary>
        /// <param name="pageNumber">The number of the page to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of bookings to include per page (default is 10).</param>
        /// <returns>A paginated result containing <see cref="UserBookingsDto"/> items.</returns>
        /// <response code="200">Returns the paginated list of user's bookings.</response>
        /// <response code="401">Returned if the user ID could not be resolved from the token claims.</response>
        [HttpGet("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.Client)]
        public async Task<ActionResult<PagedResult<UserBookingsDto>>> GetUserBookings([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authorized");

            var result = await _bookingService.GetUserBookingsAsync(pageNumber, pageSize, userId);
            return Ok(new PagedResult<UserBookingsDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                Items = result.Items
            });
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
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            var bookingDto = await _bookingService.GetBookingAsync(b => b.BookingId == id);
            return Ok(bookingDto);
        }

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="createDto">The booking creation data transfer object.</param>
        /// <returns>The newly created booking data transfer object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = Roles.Client)]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingCreateDto createDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new NotFoundException("User not found");

            var bookingDto = await _bookingService.CreateBookingAsync(createDto, userId);
            return CreatedAtRoute(nameof(GetBookingById), new { id = bookingDto.BookingId }, bookingDto);
        }

        [HttpPost("admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<ActionResult<BookingDto>> CreateBookingByReception([FromBody] BookingCreateByReceptionDto createDto)
        {
            var result = await _bookingService.CreateOfflineBookingAsync(createDto);
            return Ok(result);
        }

        [HttpPatch("{id:int}/update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDto updateDto)
        {
            await _bookingService.UpdateBookingAsync(id, updateDto);
            return NoContent();
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
        [Authorize]
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
        [Authorize(Roles = Roles.AdminRoles)]
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
        [Authorize(Roles = Roles.AdminRoles)]
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
        [Authorize(Roles = Roles.AdminRoles)]
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
            var bookingDto = await _bookingService.UpdateBookingStatusAsync(id, newStatus);
            return Ok(bookingDto);
        }
    }
}
