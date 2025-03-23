using Newtonsoft.Json;

namespace MiniHotel.Application.DTOs
{
    public class LiqPayData
    {
        [JsonProperty("order_id")]
        public string OrderId { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
    }
}
