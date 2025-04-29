using AutoMapper;
using LiqPay.SDK;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Exceptions;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Entities;
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
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMapper _mapper;

        public PaymentService(IInvoiceRepository invoiceRepository, IConfiguration configuration,
                              IMapper mapper, IInvoiceService invoiceService, ILogger<PaymentService> logger)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceService = invoiceService;
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
                throw new BadRequestException("Рахунок уже оплачено");
            }

            var orderId = $"{invoiceId}-{DateTime.UtcNow}";

            var request = new LiqPayRequest
            {
                Version = 3,
                PublicKey = _configuration["LiqPay:PublicKey"],
                Action = LiqPayRequestAction.Pay,
                Amount = Convert.ToDouble(invoice.AmountDue),
                Currency = "UAH",
                Description = description,
                OrderId = orderId,
                ServerUrl = _configuration["LiqPay:ServerUrl"],
                ResultUrl = _configuration["LiqPay:ResultUrl"],
                IsSandbox = true
            };

            var formHtml = _liqPayClient.CNBForm(request);
            return await Task.FromResult(formHtml);
        }

        public async Task<InvoiceDto> PayOfflineAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId, includeProps)
                          ?? throw new KeyNotFoundException("Invoice not found");

            return await AddPaymentAsync(invoiceId, invoice.AmountDue, PaymentMethod.OnSite);
        }

        public async Task ProcessCallbackAsync(LiqPayCallbackDto dto)
        {
            string expectedSign = _liqPayClient.CreateSignature(dto.Data);
            Console.WriteLine(expectedSign);

            if (!expectedSign.Equals(dto.Signature.Trim(), StringComparison.Ordinal))
                throw new BadRequestException($"Invalid LiqPay signature (expected={expectedSign})");

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(dto.Data));
            Console.WriteLine(json);

            var response = JsonConvert.DeserializeObject<LiqPayData>(json)
                ?? throw new BadRequestException("Invalid callback data");

            var orderParts = response.OrderId.Split('-');

            if (orderParts.Length < 2 || !int.TryParse(orderParts[0], out var invoiceId))
                throw new BadRequestException("Invalid OrderId in callback");

            if (response.Status.Equals(LiqPayResponseStatus.Success.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                await AddPaymentAsync(
                    invoiceId, Convert.ToDecimal(response.Amount),
                    PaymentMethod.Online, response.PaymentId.ToString());
            }
            else
            {
                throw new InvalidOperationException($"Payment failed: {response.Status}");
            }

            _logger.LogInformation("LiqPay callback received for Invoice {InvoiceId}: status={Status}", invoiceId, response.Status);
        }

        public async Task<InvoiceDto> AddPaymentAsync(int invoiceId, decimal amount, PaymentMethod method, string? externalId = null)
        {
            var invoice = await _invoiceRepository.GetAsync(i => i.InvoiceId == invoiceId, includeProps + ",Payments")
                          ?? throw new KeyNotFoundException("Invoice not found");

            if (amount <= 0 || amount > invoice.AmountDue)
            {
                throw new BadRequestException("Invalid payment amount");
            }

            invoice.Payments.Add(new Payment
            {
                InvoiceId = invoice.InvoiceId,
                Amount = amount,
                Method = method,
                ExternalId = externalId,
                PaidAt = DateTime.UtcNow
            });

            invoice.PaidAmount += amount;

            await _invoiceRepository.UpdateAsync(invoice);
            await _invoiceService.RecalculateAsync(invoice.InvoiceId);
            return _mapper.Map<InvoiceDto>(invoice);
        }
    }
}
