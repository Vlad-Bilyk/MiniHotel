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
using Newtonsoft.Json;
using System.Text;

namespace MiniHotel.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private const string includeProps = "InvoiceItems,InvoiceItems.Service";

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
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId, includeProps)
                          ?? throw new KeyNotFoundException("Invoice not found");

            if (invoice.Status == InvoiceStatus.Paid)
            {
                throw new InvalidOperationException("Рахунок уже оплачено");
            }

            var request = new LiqPayRequest
            {
                Version = 3,
                PublicKey = _configuration["LiqPay:PublicKey"],
                Action = LiqPayRequestAction.Pay,
                Amount = Convert.ToDouble(invoice.TotalAmount),
                Currency = "UAH",
                Description = description,
                OrderId = invoiceId.ToString(),
                ServerUrl = _configuration["LiqPay:ServerUrl"],
                ResultUrl = _configuration["LiqPay:ResultUrl"],
                IsSandbox = true
            };

            var formHtml = _liqPayClient.CNBForm(request);
            return await Task.FromResult(formHtml);
        }

        public async Task<InvoiceDto> MarkPaidOfflineAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId, includeProps)
                          ?? throw new KeyNotFoundException("Invoice not found");

            if (invoice.Status == InvoiceStatus.Paid)
            {
                throw new InvalidOperationException("Рахунок уже оплачено");
            }

            invoice.Status = InvoiceStatus.Paid;
            await _invoiceRepository.UpdateAsync(invoice);
            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task ProcessCallbackAsync(LiqPayCallbackDto dto)
        {
            string expectedSign = _liqPayClient.CreateSignature(dto.Data);
            Console.WriteLine(expectedSign);

            if (!expectedSign.Equals(dto.Signature.Trim(), StringComparison.Ordinal))
                throw new InvalidOperationException($"Invalid LiqPay signature (expected={expectedSign})");

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(dto.Data));
            Console.WriteLine(json);
            var response = JsonConvert.DeserializeObject<LiqPayData>(json);

            if (response is null)
            {
                throw new InvalidOperationException("Invalid callback data");
            }

            if (!int.TryParse(response.OrderId, out var invoiceId))
                throw new InvalidOperationException("Invalid OrderId in callback");

            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId, includeProps)
                          ?? throw new KeyNotFoundException($"Invoice not found");

            _logger.LogInformation("LiqPay callback received for Invoice {InvoiceId}: status={Status}", invoiceId, response.Status);

            invoice.Status = response.Status.Equals("success", StringComparison.OrdinalIgnoreCase)
                ? InvoiceStatus.Paid
                : InvoiceStatus.Cancelled;

            _logger.LogInformation("Invoice {InvoiceId} status updated to {NewStatus}", invoiceId, invoice.Status);

            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task<InvoiceDto> MarkRefundAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId, includeProps)
                          ?? throw new KeyNotFoundException("Invoice not found");

            if (invoice.Status != InvoiceStatus.Paid)
            {
                throw new InvalidOperationException("Рахунок ще не оплачено");
            }

            _logger.LogInformation("Refunding Invoice {InvoiceId}", invoiceId);
            invoice.Status = InvoiceStatus.Refunded;
            await _invoiceRepository.UpdateAsync(invoice);
            return _mapper.Map<InvoiceDto>(invoice);
        }
    }
}
