namespace MiniHotel.Application.DTOs
{
    public class AuthenticationResultDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
