using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;

namespace MiniHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypesController : ControllerBase
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IMapper _mapper;

        public RoomTypesController(IRoomTypeRepository roomTypeRepository, IMapper mapper)
        {
            _roomTypeRepository = roomTypeRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all hotel room types.
        /// </summary>
        /// <returns>A collection of room type DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoomTypeDto>>> GetRoomTypes()
        {
            IEnumerable<RoomType> roomTypes = await _roomTypeRepository.GetAllAsync();
            return Ok(_mapper.Map<List<RoomTypeDto>>(roomTypes));
        }

        /// <summary>
        /// Retrieves a room type by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the room type.</param>
        /// <returns>The room type DTO if found.</returns>
        /// <response code="200">Returns the room type DTO.</response>
        /// <response code="400">If the request parameters are invalid.</response>
        /// <response code="404">If the room is not found.</response>
        [HttpGet("{id:int}", Name = "GetRoomType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetRoomType(int id)
        {
            RoomType roomType = await _roomTypeRepository.GetAsync(rt => rt.RoomTypeId == id);
            if (roomType == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<RoomTypeDto>(roomType));
        }

        /// <summary>
        /// Creates a new room type.
        /// </summary>
        /// <param name="createDto">The room type creation data transfer object.</param>
        /// <returns>The created room type DTO.</returns>
        /// <response code="201">Returns the newly created room type.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="500">If an internal server error occurs during creation.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoomTypeDto>> CreateRoomType([FromBody] RoomTypeUpsertDto createDto)
        {
            RoomType roomType = _mapper.Map<RoomType>(createDto);
            await _roomTypeRepository.CreateAsync(roomType);
            RoomTypeDto dto = _mapper.Map<RoomTypeDto>(roomType);
            return CreatedAtRoute(nameof(GetRoomType), new { id = roomType.RoomTypeId }, dto);
        }

        /// <summary>
        /// Updates an existing room type.
        /// </summary>
        /// <param name="id">The unique identifier of the room type to update.</param>
        /// <param name="updateDto">The room type update data transfer object.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">Room Type updated successfully.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="404">If the room type is not found.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRoomType(int id, [FromBody] RoomTypeUpsertDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }

            var existingRoomType = await _roomTypeRepository.GetAsync(rt => rt.RoomTypeId == id);
            _mapper.Map(updateDto, existingRoomType);
            await _roomTypeRepository.UpdateAsync(existingRoomType);
            return NoContent();
        }

        /// <summary>
        /// Deletes a room type by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the room type to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="204">Room type deleted successfully.</response>
        /// <response code="404">If the room type is not found.</response>
        /// <response code="400">If the request is invalid.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            RoomType roomType = await _roomTypeRepository.GetAsync(rt => rt.RoomTypeId == id);
            if (roomType == null)
            {
                return NotFound();
            }
            await _roomTypeRepository.RemoveAsync(roomType);
            return NoContent();
        }
    }
}
