using Microsoft.AspNetCore.Hosting;
using MiniHotel.Application.Common.Helpers;
using MiniHotel.Application.Interfaces.IService;

namespace MiniHotel.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private const string UploadsFolder = "uploads";
        private const string RoomTypesFolder = "roomtypes";
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _env = webHostEnvironment;
        }

        public async Task<string> SaveRoomTypeImageAsync(Stream stream, string fileName, string roomCategory, string? existingImageUrl = null)
        {
            ValidateInput(stream, roomCategory);

            var webRoot = GetWebRootPath();
            var uploadDir = CreateUploadDirectory(webRoot);

            DeleteExistingImage(webRoot, existingImageUrl);

            var safeBase = BuildSlug(roomCategory);
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = BuildUniqueFileName(uploadDir, safeBase, extension);
            var fullPath = Path.Combine(uploadDir, uniqueFileName);

            await WriteToFileAsync(stream, fullPath);

            return "/" + Path.Combine(UploadsFolder, RoomTypesFolder, uniqueFileName).Replace('\\', '/');
        }

        private static void ValidateInput(Stream stream, string roomCategory)
        {
            if (stream == null || stream.Length == 0)
                throw new ArgumentException("Invalid file stream");
            if (string.IsNullOrWhiteSpace(roomCategory))
                throw new ArgumentException("Room category must not be empty.");
        }

        private string GetWebRootPath() =>
            _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        private static string CreateUploadDirectory(string webRoot)
        {
            var path = Path.Combine(webRoot, UploadsFolder, RoomTypesFolder);
            Directory.CreateDirectory(path);
            return path;
        }

        private static void DeleteExistingImage(string webRoot, string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            var relative = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRoot, relative);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private static string BuildSlug(string category)
        {
            var slug = TransliterationHelper.Transliterate(category).ToLowerInvariant();

            foreach (var c in new[] { ' ', '.', '/', '\\' })
                slug = slug.Replace(c, '-');

            slug = string.Join('-', slug.Split('-', StringSplitOptions.RemoveEmptyEntries));
            return slug;
        }

        private static string BuildUniqueFileName(string dir, string baseName, string ext)
        {
            var pattern = $"{baseName}*{ext}";
            var existingCount = Directory.GetFiles(dir, pattern).Length;
            return $"{baseName}-{existingCount + 1}{ext}";
        }

        private static async Task WriteToFileAsync(Stream source, string destinationPath)
        {
            await using var dest = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await source.CopyToAsync(dest);
        }
    }
}
