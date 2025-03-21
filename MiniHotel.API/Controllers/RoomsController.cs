using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;

namespace MiniHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        public RoomsController(IRoomRepository roomRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            IEnumerable<Room> rooms = await _roomRepository.GetAllAsync();
            return Ok(_mapper.Map<List<RoomDto>>(rooms));
        }

        [HttpGet("{id:int}", Name = "GetRoomById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetRoomById(int id)
        {
            Room room = await _roomRepository.GetAsync(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RoomDto>(room));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] RoomUpsertDto createDto)
        {
            Room room = _mapper.Map<Room>(createDto);
            await _roomRepository.CreateAsync(room);
            RoomDto roomDto = _mapper.Map<RoomDto>(room);
            return CreatedAtRoute(nameof(GetRoomById), new { id = room.RoomId }, roomDto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSubscriber(int id, [FromBody] RoomUpsertDto updateDto)
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

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSubscriber(int id)
        {
            Room room = await _roomRepository.GetAsync(r => r.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }
            await _roomRepository.RemoveAsync(room);
            return NoContent();
        }

        [HttpGet("{startDate:datetime}, {endDate:datetime}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            IEnumerable<Room> rooms = await _roomRepository.GetAvailableRoomsAsync(startDate, endDate);
            return Ok(_mapper.Map<List<RoomDto>>(rooms));
        }
    }
}
