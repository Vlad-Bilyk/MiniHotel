using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Constants;

namespace MiniHotel.API.Controllers
{
    /// <summary>
    /// Controller for managing users. Provides endpoints to retrieve, update, and delete user information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Manager)]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A collection of user DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(_mapper.Map<List<UserDto>>(users));
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user DTO if found.</returns>
        /// <response code="200">Returns the user DTO.</response>
        /// <response code="400">If the provided identifier is invalid.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUserById(string id)
        {
            var user = await _userRepository.GetAsync(r => r.UserId == id);
            return Ok(_mapper.Map<UserDto>(user));
        }

        /// <summary>
        /// Updates a user's information.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="updateDto">The user update data transfer object.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">User updated successfully.</response>
        /// <response code="400">If the request data is invalid.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto updateDto)
        {
            var existingUser = await _userRepository.GetAsync(r => r.UserId == id);
            _mapper.Map(updateDto, existingUser);
            await _userRepository.UpdateAsync(existingUser);
            return NoContent();
        }
    }
}
