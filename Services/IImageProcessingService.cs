
using FotosAPI.Models;
using FotosAPI.ViewModel;

namespace FotosAPI.Services
{
    public interface IImageProcessingService
    {
        (int width, int height) ProcessAndSaveImage(Stream imageStream, string filePath, int quality);
        void CreateThumbnail(string originalFilePath, string thumbnailPath, int thumbnailWidth);

        Photo AllImageProcess(PhotoViewModel photoView, string uploadedBy, string applicationId);
    }
}
