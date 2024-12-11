
using FotosAPI.Models;


namespace FotosAPI.Services
{
    public class ThumbnailService : IThumbnailService
    {
        private readonly IPhotoRepository _photoRepository;
        public ThumbnailService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }
        public async Task<(byte[] dataBytes, Photo photo)> GetViewThumbnails(int id)
        {
            // Busca o obj no repositório
            var pic = _photoRepository.Get(id);
            if (pic == null)
                throw new FileNotFoundException("Thumbnail não encontrada.");

            // Lê o conteúdo da imagem em bytes
            var dataBytes = await File.ReadAllBytesAsync(pic.ThumbPath);

            // Ajuste horário local no objeto Photo
            var localTimeZone = TimeZoneInfo.Local;
            pic.UploadedAt = TimeZoneInfo.ConvertTimeFromUtc(pic.UploadedAt, localTimeZone);

            return (dataBytes, pic);
        }
    }

    public interface IThumbnailService
    {
        Task<(byte[] dataBytes, Photo photo)> GetViewThumbnails(int id);
    }
}
