using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Constants;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;

namespace MiniHotel.API.Controllers
{
    /// <summary>
    /// Controller for managing hotel rooms. Provides endpoints to retrieve room information,
    /// create new rooms, update existing rooms, delete rooms, and retrieve available rooms.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IMapper _mapper;

        public RoomsController(IRoomRepository roomRepository, IMapper mapper, IRoomTypeRepository roomTypeRepository)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
            _roomTypeRepository = roomTypeRepository;
        }

        /// <summary>
        /// Retrieves all hotel rooms.
        /// </summary>
        /// <returns>A collection of room DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            IEnumerable<Room> rooms = await _roomRepository.GetAllAsync(includeProperties: "RoomType");
            return Ok(_mapper.Map<List<RoomDto>>(rooms));
        }

        /// <summary>
        /// Retrieves a room by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the room.</param>
        /// <returns>The room DTO if found.</returns>
        /// <response code="200">Returns the room DTO.</response>
        /// <response code="400">If the request parameters are invalid.</response>
        /// <response code="404">If the room is not found.</response>
        [HttpGet("{id:int}", Name = "GetRoomById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<ActionResult> GetRoomById(int id)
        {
            Room room = await _roomRepository.GetAsync(r => r.RoomId == id, includeProperties: "RoomType");
            if (room == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<RoomDto>(room));
        }

        /// <summary>
        /// Creates a new room.
        /// </summary>
        /// <param name="createDto">The room creation data transfer object.</param>
        /// <returns>The created room DTO.</returns>
        /// <response code="201">Returns the newly created room.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="500">If an internal server error occurs during creation.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] RoomUpsertDto createDto)
        {
            var roomType = await _roomTypeRepository.GetAsync(rt => rt.RoomTypeId == createDto.RoomTypeId);
            if (roomType is null)
            {
                return BadRequest("Room type not found.");
            }

            var room = new Room
            {
                RoomNumber = createDto.RoomNumber,
                RoomTypeId = roomType.RoomTypeId,
                RoomStatus = createDto.RoomStatus
            };

            await _roomRepository.CreateAsync(room);
            RoomDto roomDto = _mapper.Map<RoomDto>(room);
            return CreatedAtRoute(nameof(GetRoomById), new { id = room.RoomId }, roomDto);
        }

        /// <summary>
        /// Updates an existing room.
        /// </summary>
        /// <param name="id">The unique identifier of the room to update.</param>
        /// <param name="updateDto">The room update data transfer object.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">Room updated successfully.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="404">If the room is not found.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomUpsertDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }

            var existingRoom = await _roomRepository.GetAsync(r => r.RoomId == id);
            _mapper.Map(updateDto, existingRoom);
            await _roomRepository.UpdateAsync(existingRoom);
            return NoContent();
        }

        /// <summary>
        /// Retrieves available rooms for the specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the desired booking period.</param>
        /// <param name="endDate">The end date of the desired booking period.</param>
        /// <param name="ignoreBookingId">Optional. Booking ID to ignore when checking room availability (used for editing existing bookings).</param>
        /// <returns>A collection of available room DTOs.</returns>
        /// <response code="200">Returns the list of available rooms.</response>
        /// <response code="400">If the date range is invalid.</response>
        [HttpGet("available/{startDate:datetime}/{endDate:datetime}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRooms(DateTime startDate, DateTime endDate, int? ignoreBookingId = null)
        {
            if (endDate < startDate)
            {
                return BadRequest("End date must be greater than start date.");
            }

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            var rooms = await _roomRepository.GetAvailableRoomsAsync(startDate, endDate, ignoreBookingId);
            return Ok(_mapper.Map<List<RoomDto>>(rooms));
        }

        /// <summary>
        /// Updates the status of a room.
        /// </summary>
        /// <param name="id">The unique identifier of the room.</param>
        /// <param name="newStatus">The new status for the room.</param>
        /// <returns>The updated room DTO.</returns>
        /// <response code="200">Returns the updated room.</response>
        /// <response code="404">If the room is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ActionResult<RoomDto>> UpdateRoomStatus(int id, RoomStatus newStatus)
        {
            var roomDto = await _roomRepository.UpdateStatusAsync(id, newStatus);
            return Ok(roomDto);
        }
    }
}
