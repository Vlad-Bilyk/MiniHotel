using Newtonsoft.Json;

namespace MiniHotel.Application.DTOs
{
    public class LiqPayData
    {
        [JsonProperty("order_id")]
        public string OrderId { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("payment_id")]
        public int PaymentId { get; set; }
    }
}
