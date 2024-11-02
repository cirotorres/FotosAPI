using FotosAPI.Models;

namespace FotosAPI.Services
{
    public class AllListService : IAllListService
    {
        private readonly IPhotoRepository _photoRepository;

        public AllListService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }
        public List<PhotoDTO> AllList()
        {
            // Lista todos os objetos cadastrados
            var obj = _photoRepository.GetAll().ToList();

            if (obj == null || !obj.Any())
                throw new KeyNotFoundException("Lista vazia.");

            var localTimeZone = TimeZoneInfo.Local;

            var photoreturnDTO = obj.Select(photo => new PhotoDTO(

                Id: photo.Id,
                Title: photo.Title,
                UploadedBy: photo.UploadedBy,
                ApplicationId: photo.ApplicationId,
                UploadedAt: TimeZoneInfo.ConvertTimeFromUtc(photo.UploadedAt, localTimeZone)

                )).ToList();


            return photoreturnDTO;
        }
    }

    public interface IAllListService
    {
        List<PhotoDTO> AllList();
    }

}
