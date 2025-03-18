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
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            IEnumerable<Room> rooms = await _roomRepository.GetAllAsync();
            return Ok(_mapper.Map<List<RoomDto>>(rooms));
        }

        [HttpGet("{id:int}", Name = "GetRoomById")]
        public async Task<ActionResult> GetRoomById(int id)
        {
            Room room = await _roomRepository.GetAsync(r => r.RoomId == id);
            return Ok(_mapper.Map<RoomDto>(room));
        }

        [HttpPost]
        public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] RoomCreateDto createDto)
        {
            Room room = _mapper.Map<Room>(createDto);
            await _roomRepository.CreateAsync(room);
            RoomDto roomDto = _mapper.Map<RoomDto>(room);
            return CreatedAtRoute(nameof(GetRoomById), new { id = room.RoomId }, roomDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSubscriber(int id, [FromBody] RoomDto updateDto)
        {
            if (updateDto == null || id != updateDto.RoomId)
            {
                return BadRequest();
            }

            Room room = _mapper.Map<Room>(updateDto);
            await _roomRepository.UpdateAsync(room);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSubscriber(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            Room room = await _roomRepository.GetAsync(r => r.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }
            await _roomRepository.RemoveAsync(room);
            return NoContent();
        }
    }
}
