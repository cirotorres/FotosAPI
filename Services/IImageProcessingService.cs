
namespace FotosAPI.Services
{
    public interface IImageProcessingService
    {
        (int width, int height) ProcessAndSaveImage(Stream imageStream, string filePath, int quality);
        void CreateThumbnail(string originalFilePath, string thumbnailPath, int thumbnailWidth);
    }
}
