using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Constants;
using MiniHotel.Domain.Entities;

namespace MiniHotel.API.Controllers
{
    /// <summary>
    /// Controller for managing additional services provided by the hotel.
    /// Provides endpoints to retrieve, create, update, and delete services.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;

        public ServicesController(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all additional services.
        /// </summary>
        /// <returns>A collection of service DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            IEnumerable<Service> services = await _serviceRepository.GetAllAsync();
            return Ok(_mapper.Map<List<ServiceDto>>(services));
        }

        /// <summary>
        /// Retrieves a specific service by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the service.</param>
        /// <returns>The service DTO if found.</returns>
        /// <response code="200">Returns the service.</response>
        /// <response code="400">If the request parameters are invalid.</response>
        /// <response code="404">If the service is not found.</response>
        [HttpGet("{id:int}", Name = "GetServiceById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<ActionResult> GetServiceById(int id)
        {
            Service service = await _serviceRepository.GetAsync(r => r.ServiceId == id);
            return Ok(_mapper.Map<ServiceDto>(service));
        }

        /// <summary>
        /// Creates a new additional service.
        /// </summary>
        /// <param name="createDto">The service creation data transfer object.</param>
        /// <returns>The created service DTO.</returns>
        /// <response code="201">Returns the newly created service.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="500">If an internal server error occurs during creation.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ActionResult<ServiceDto>> CreateService([FromBody] ServiceUpsertDto createDto)
        {
            Service service = _mapper.Map<Service>(createDto);
            await _serviceRepository.CreateAsync(service);
            ServiceDto serviceDto = _mapper.Map<ServiceDto>(service);
            return CreatedAtRoute(nameof(GetServiceById), new { id = service.ServiceId }, serviceDto);
        }

        /// <summary>
        /// Updates an existing service.
        /// </summary>
        /// <param name="id">The unique identifier of the service to update.</param>
        /// <param name="updateDto">The service update data transfer object.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">Service updated successfully.</response>
        /// <response code="400">If the request data is invalid.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceUpsertDto updateDto)
        {
            var existingService = await _serviceRepository.GetAsync(r => r.ServiceId == id);
            _mapper.Map(updateDto, existingService);
            await _serviceRepository.UpdateAsync(existingService);
            return NoContent();
        }

        /// <summary>
        /// Deactivates the service specified by the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the service to deactivate.</param>
        /// <returns>No content if the service was successfully deactivated.</returns>
        /// <response code="204">Service deactivated successfully.</response>
        /// <response code="404">Service with the specified ID was not found.</response>
        [HttpPatch("{id:int}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _serviceRepository.Deactivate(id);
            return NoContent();
        }

        /// <summary>
        /// Reactivates the service specified by the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the service to reactivate.</param>
        /// <returns>No content if the service was successfully reactivated.</returns>
        /// <response code="204">Service reactivated successfully.</response>
        /// <response code="404">Service with the specified ID was not found.</response>
        [HttpPatch("{id:int}/reactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reactivate(int id)
        {
            await _serviceRepository.Reactivate(id);
            return NoContent();
        }
    }
}
