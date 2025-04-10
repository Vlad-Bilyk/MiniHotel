using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;

namespace MiniHotel.API.Controllers
{
    /// <summary>
    /// Controller for managing payment operations including online payments, offline payments,
    /// handling callbacks from LiqPay, and processing refunds.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PayOnline(int invoiceId)
        {
            return await ExecuteSafeAsync(async () =>
            {
                var paymentForm = await _paymentService.CreatePaymentUrlAsync(invoiceId, $"Оплата рахунку №{invoiceId}");
                return Content(paymentForm, "text/html");
            }, invoiceId, "Online payment creation");
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
        public async Task<ActionResult<InvoiceDto>> PayOffline([FromRoute] int invoiceId)
        {
            return await ExecuteSafeAsync(async () =>
            {
                var updatedInvoice = await _paymentService.MarkPaidOfflineAsync(invoiceId);
                return Ok(updatedInvoice);
            }, invoiceId, "Offline payment");
        }

        /// <summary>
        /// Receives and processes LiqPay callback data.
        /// </summary>
        /// <param name="dto">The LiqPay callback data transfer object.</param>
        /// <returns>An HTTP 200 OK response if processed successfully.</returns>
        /// <response code="200">If the callback is processed successfully.</response>
        /// <response code="400">If the callback data format is invalid.</response>
        /// <response code="404">If the resource related to the callback is not found.</response>
        [HttpPost("callback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Callback([FromForm] LiqPayCallbackDto dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Data) || string.IsNullOrWhiteSpace(dto.Signature))
            {
                _logger.LogWarning("Invalid callback data received from LiqPay");
                return BadRequest(new { error = "Invalid callback data format" });
            }

            return await ExecuteSafeAsync(async () =>
            {
                await _paymentService.ProcessCallbackAsync(dto);
                return Ok();
            }, description: "LiqPay callback processing");
        }

        /// <summary>
        /// Processes a refund request for a specific invoice.
        /// </summary>
        /// <param name="invoiceId">The unique identifier of the invoice for which a refund is requested.</param>
        /// <returns>The updated invoice DTO after processing the refund.</returns>
        /// <response code="200">Returns the updated invoice after refund.</response>
        /// <response code="400">If the refund process fails due to invalid data.</response>
        /// <response code="404">If the invoice is not found.</response>
        [HttpPost("{invoiceId:int}/refund")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InvoiceDto>> Refund([FromRoute] int invoiceId)
        {
            return await ExecuteSafeAsync(async () =>
            {
                var updatedInvoice = await _paymentService.MarkRefundAsync(invoiceId);
                return Ok(updatedInvoice);
            }, invoiceId, "Refund process");
        }

        /// <summary>
        /// Executes a function safely while handling common exceptions and logging.
        /// </summary>
        /// <param name="action">The asynchronous function to execute.</param>
        /// <param name="invoiceId">Optional invoice identifier related to the action.</param>
        /// <param name="description">A description of the operation being performed.</param>
        /// <returns>An <see cref="ActionResult"/> representing the result of the operation.</returns>
        private async Task<ActionResult> ExecuteSafeAsync(Func<Task<object>> action, int? invoiceId = null, string description = "")
        {
            try
            {
                var result = await action();
                if (result is ActionResult ar)
                {
                    return ar;
                }
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Invoice {InvoiceId} not found during operation: {Description}", invoiceId, description);
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business logic error during operation: {Description}, InvoiceId: {InvoiceId}", description, invoiceId);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during operation: {Description}, InvoiceId: {InvoiceId}", description, invoiceId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error" });
            }
        }
    }
}
