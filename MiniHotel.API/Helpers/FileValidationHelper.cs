using MiniHotel.Application.Exceptions;

namespace MiniHotel.API.Helpers
{
    public static class FileValidationHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public static void ValidationImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BadRequestException("No file uploaded.");
            }

            if (file.Length > MaxFileSize)
            {
                throw new BadRequestException("File is too large. Maximum allowed size is 5 MB.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !AllowedExtensions.Contains(fileExtension))
            {
                throw new BadRequestException("Invalid file type. Only JPG and PNG files are allowed.");
            }
        }
    }
}
