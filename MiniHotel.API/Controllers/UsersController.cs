using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;

namespace MiniHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            IEnumerable<User> users = await _userRepository.GetAllAsync();
            return Ok(_mapper.Map<List<UserDto>>(users));
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUserById(string id)
        {
            User user = await _userRepository.GetAsync(r => r.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }

            var existingUser = await _userRepository.GetAsync(r => r.UserId == id);

            if (existingUser == null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, existingUser);
            await _userRepository.UpdateAsync(existingUser);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            User user = await _userRepository.GetAsync(r => r.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            await _userRepository.RemoveAsync(user);
            return NoContent();
        }
    }
}
