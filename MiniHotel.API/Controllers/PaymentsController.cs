using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Constants;

namespace MiniHotel.API.Controllers
{
    /// <summary>
    /// Controller for managing payment operations including online payments, offline payments,
    /// handling callbacks from LiqPay, and processing refunds.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Initiates an online payment by generating a payment URL.
        /// </summary>
        /// <param name="invoiceId">The unique identifier of the invoice to be paid.</param>
        /// <returns>An HTML content containing the payment form.</returns>
        /// <response code="200">Returns the HTML form for online payment.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the invoice is not found.</response>
        [HttpPost("{invoiceId:int}/online")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult> PayOnline(int invoiceId)
        {
            var paymentForm = await _paymentService.CreatePaymentUrlAsync(invoiceId, $"Оплата рахунку №{invoiceId}");
            return Content(paymentForm, "text/html");
        }

        /// <summary>
        /// Processes an offline payment, marking the invoice as paid offline.
        /// </summary>
        /// <param name="invoiceId">The unique identifier of the invoice.</param>
        /// <returns>The updated invoice DTO reflecting offline payment.</returns>
        /// <response code="200">Returns the updated invoice.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="404">If the invoice is not found.</response>
        [HttpPost("{invoiceId:int}/offline")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = Roles.AdminRoles)]
        public async Task<ActionResult<InvoiceDto>> PayOffline([FromRoute] int invoiceId)
        {
            var updatedInvoice = await _paymentService.PayOfflineAsync(invoiceId);
            return Ok(updatedInvoice);
        }

        /// <summary>
        /// Receives and processes LiqPay callback data.
        /// </summary>
        /// <param name="dto">The LiqPay callback data transfer object.</param>
        /// <returns>An HTTP 200 OK response if processed successfully.</returns>
        /// <response code="200">If the callback is processed successfully.</response>
        /// <response code="400">If the callback data format is invalid.</response>
        [HttpPost("callback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult> Callback([FromForm] LiqPayCallbackDto dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Data) || string.IsNullOrWhiteSpace(dto.Signature))
            {
                _logger.LogWarning("Invalid callback data received from LiqPay");
                throw new BadHttpRequestException("Invalid callback data format");
            }

            await _paymentService.ProcessCallbackAsync(dto);
            return Ok();
        }
    }
}
