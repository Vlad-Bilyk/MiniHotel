using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiniHotel.Tests.Integration
{
    public static class JsonHelpers
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true,
        };
    }
}
