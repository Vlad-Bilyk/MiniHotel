using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Enums;

namespace MiniHotel.API.Controllers
{
    /// <summary>
    /// Controller for managing invoices. Provides endpoints to retrieve, create, and update invoices as well as manage invoice items.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Retrieves all invoices.
        /// </summary>
        /// <returns>A collection of invoice DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        /// <summary>
        /// Retrieves an invoice by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the invoice.</param>
        /// <returns>An invoice DTO if found.</returns>
        /// <response code="200">Returns the invoice.</response>
        /// <response code="404">If the invoice is not found.</response>
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

        /// <summary>
        /// Creates an invoice for a specific booking.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking.</param>
        /// <returns>The created invoice DTO.</returns>
        /// <response code="201">Returns the newly created invoice.</response>
        [HttpPost("{bookingId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<InvoiceDto>> CreateInvoice(int bookingId)
        {
            var dto = await _invoiceService.CreateInvoiceForBookingAsync(bookingId);
            return CreatedAtAction(nameof(GetInvoice), new { bookingId }, dto);
        }

        /// <summary>
        /// Adds an invoice item to a specific booking's invoice.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking.</param>
        /// <param name="createItem">The data needed to create the invoice item.</param>
        /// <returns>The updated invoice DTO with the new item included.</returns>
        /// <response code="201">Returns the invoice with the new item added.</response>
        [HttpPost("{bookingId}/items")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<InvoiceDto>> AddInvoiceItem(int bookingId, InvoiceItemCreateDto createItem)
        {
            var dto = await _invoiceService.AddItemAsync(bookingId, createItem);
            return Ok(dto);
        }

        /// <summary>
        /// Removes an invoice item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the invoice item.</param>
        /// <returns>A response with no content.</returns>
        /// <response code="204">If the invoice item is successfully removed.</response>
        [HttpDelete("items/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> RemoveInvoiceItem(int id)
        {
            await _invoiceService.RemoveItemAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Updates the status of an invoice.
        /// </summary>
        /// <param name="id">The unique identifier of the invoice.</param>
        /// <param name="status">The new status to be applied.</param>
        /// <returns>The updated invoice DTO.</returns>
        /// <response code="200">Returns the updated invoice.</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<InvoiceDto>> UpdateStatus(int id, InvoiceStatus status)
        {
            var invoiceDto = await _invoiceService.UpdateStatusAsync(id, status);
            return Ok(invoiceDto);
        }
    }
}
