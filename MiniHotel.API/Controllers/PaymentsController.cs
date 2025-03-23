using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;

namespace MiniHotel.API.Controllers
{
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
            }, invoiceId, "Створення онлайн-платежу");
        }

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
            }, invoiceId, "офлайн-оплата");
        }

        [HttpPost("callback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Callback([FromForm] LiqPayCallbackDto dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Data) || string.IsNullOrWhiteSpace(dto.Signature))
            {
                _logger.LogWarning("Некоректні callback-дані від LiqPay");
                return BadRequest(new { error = "Invalid callback data format" });
            }

            return await ExecuteSafeAsync(async () =>
            {
                await _paymentService.ProcessCallbackAsync(dto);
                return Ok();
            }, description: "Обробка LiqPay callback");
        }

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
            }, invoiceId, "повернення коштів");
        }

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
                _logger.LogWarning(ex, "Рахунок {InvoiceId} не знайдено при виконанні операції: {Description}", invoiceId, description);
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Помилка бізнес-логіки при виконанні операції: {Description}, InvoiceId: {InvoiceId}", description, invoiceId);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неочікувана помилка під час операції: {Description}, InvoiceId: {InvoiceId}", description, invoiceId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error" });
            }
        }
    }
}
