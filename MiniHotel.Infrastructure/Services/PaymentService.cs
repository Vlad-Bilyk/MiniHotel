using AutoMapper;
using LiqPay.SDK;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Enums;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MiniHotel.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly LiqPayClient _liqPayClient;
        private readonly IConfiguration _configuration;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMapper _mapper;

        public PaymentService(IInvoiceRepository invoiceRepository, IConfiguration configuration,
                              IMapper mapper, ILogger<PaymentService> logger)
        {
            _invoiceRepository = invoiceRepository;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;

            var publicKey = _configuration["LiqPay:PublicKey"];
            var privateKey = _configuration["LiqPay:PrivateKey"];

            _liqPayClient = new LiqPayClient(publicKey, privateKey);
        }

        public async Task<string> CreatePaymentUrlAsync(int invoiceId, string description)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId)
                          ?? throw new KeyNotFoundException("Invoice not found");

            var request = new LiqPayRequest
            {
                Version = 3,
                PublicKey = _configuration["LiqPay:PublicKey"],
                Action = LiqPayRequestAction.Pay,
                Amount = Convert.ToDouble(invoice.TotalAmount),
                Currency = "UAH",
                Description = description,
                OrderId = invoiceId.ToString(),
                ResultUrl = _configuration["LiqPay:ResultUrl"],
                IsSandbox = true
            };

            var formHtml = _liqPayClient.CNBForm(request);
            return await Task.FromResult(formHtml);
        }

        public async Task<InvoiceDto> MarkPaidOfflineAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId)
                          ?? throw new KeyNotFoundException("Invoice not found");

            invoice.Status = InvoiceStatus.Paid;
            await _invoiceRepository.UpdateAsync(invoice);
            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task ProcessCallbackAsync(LiqPayCallbackDto dto)
        {
            var privateKey = _configuration["LiqPay:PrivateKey"];
            var expectedSignature = Convert.ToBase64String(SHA1.Create().ComputeHash(
                Encoding.UTF8.GetBytes($"{privateKey}{dto.Data}{privateKey}")));

            if (!expectedSignature.Equals(dto.Signature, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid LiqPay signature");
            }

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(dto.Data));
            var response = JsonSerializer.Deserialize<LiqPayResponse>(json);

            if (response is null)
            {
                throw new InvalidOperationException("Invalid callback data");
            }

            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == int.Parse(response.OrderId))
                          ?? throw new KeyNotFoundException($"Invoice not found");

            _logger.LogInformation("LiqPay callback received for Invoice {InvoiceId}: status={Status}", invoice.InvoiceId, response.Status);

            invoice.Status = response.Status switch
            {
                LiqPayResponseStatus.Success => InvoiceStatus.Paid,
                LiqPayResponseStatus.Failure => InvoiceStatus.Cancelled,
                LiqPayResponseStatus.Error => InvoiceStatus.Cancelled,
                LiqPayResponseStatus.Reversed => InvoiceStatus.Refunded,
                _ => InvoiceStatus.Pending,
            };

            _logger.LogInformation("Invoice {InvoiceId} status updated to {NewStatus}", invoice.InvoiceId, invoice.Status);

            await _invoiceRepository.UpdateAsync(invoice);
        }
    }
}
