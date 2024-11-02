
using FotosAPI.Models;
using FotosAPI.ViewModel;

namespace FotosAPI.Services
{
    public interface IImageProcessingService
    {
        Task<(int width, int height)> ProcessAndSaveImage(Stream imageStream, string filePath, int quality);
        Task CreateThumbnail(string originalFilePath, string thumbnailPath, int thumbnailWidth);

        Task<Photo> AllImageProcess(PhotoViewModel photoView, string uploadedBy, string applicationId);
    }
}
