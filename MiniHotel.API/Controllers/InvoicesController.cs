using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Enums;

namespace MiniHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetBookings()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        [HttpGet("{id:int}", Name = "GetInvoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
        {
            var invoice = await _invoiceService.GetInvoiceAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        [HttpPost("{bookingId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<InvoiceDto>> CreateInvoice(int bookingId)
        {
            var dto = await _invoiceService.CreateInvoiceForBookingAsync(bookingId);
            return CreatedAtAction(nameof(GetInvoice), new { bookingId }, dto);
        }

        [HttpPost("{bookingId}/items")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<InvoiceDto>> AddItem(int bookingId, InvoiceItemCreateDto createItem)
        {
            var dto = await _invoiceService.AddItemAsync(bookingId, createItem);
            return Ok(dto);
        }

        [HttpDelete("items/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> RemoveItem(int id)
        {
            await _invoiceService.RemoveItemAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<InvoiceDto>> UpdateStatus(int id, InvoiceStatus status)
        {
            var invoiceDto = await _invoiceService.UpdateStatusAsync(id, status);
            return Ok(invoiceDto);
        }
    }
}
