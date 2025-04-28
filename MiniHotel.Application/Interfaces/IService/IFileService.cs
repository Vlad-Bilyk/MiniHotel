namespace MiniHotel.Application.Interfaces.IService
{
    public interface IFileService
    {
        Task<string> SaveRoomTypeImageAsync(Stream stream, string fileName, string roomCategory, string? existingImageUrl = null);
    }
}
