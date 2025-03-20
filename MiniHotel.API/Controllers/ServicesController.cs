using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;

namespace MiniHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;

        public ServicesController(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            IEnumerable<Service> services = await _serviceRepository.GetAllAsync();
            return Ok(_mapper.Map<List<ServiceDto>>(services));
        }

        [HttpGet("{id:int}", Name = "GetServiceById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetServiceById(int id)
        {
            Service service = await _serviceRepository.GetAsync(r => r.ServiceId == id);

            if (service == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ServiceDto>(service));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceDto>> CreateService([FromBody] ServiceUpsertDto createDto)
        {
            Service service = _mapper.Map<Service>(createDto);
            await _serviceRepository.CreateAsync(service);
            ServiceDto serviceDto = _mapper.Map<ServiceDto>(service);
            return CreatedAtRoute(nameof(GetServiceById), new { id = service.ServiceId }, serviceDto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceUpsertDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }

            var existingService = await _serviceRepository.GetAsync(r => r.ServiceId == id);
            _mapper.Map(updateDto, existingService);
            await _serviceRepository.UpdateAsync(existingService);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteService(int id)
        {
            Service service = await _serviceRepository.GetAsync(r => r.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }
            await _serviceRepository.RemoveAsync(service);
            return NoContent();
        }
    }
}
