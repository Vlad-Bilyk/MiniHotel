using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class BookingCreateByAdminDto
    {
        [Required]
        [StringLength(4)]
        public string RoomNumber { get; set; } = default!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = default!;

        [Required, Phone]
        public string PhoneNumber { get; set; } = default!;

        [Required]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.OnSite;

    }
}
