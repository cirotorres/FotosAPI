using FotosAPI.Models;

namespace FotosAPI.Services
{
    public class AllListService : IAllListService
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AllListService(IPhotoRepository photoRepository, IHttpContextAccessor httpContextAccessor)
        {
            _photoRepository = photoRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public List<PhotoDTO> AllList()
        {
            // Lista todos os objetos cadastrados
            var obj = _photoRepository.GetAll().ToList();

            if (obj == null || !obj.Any())
                throw new KeyNotFoundException("Lista vazia.");

            var localTimeZone = TimeZoneInfo.Local;

            var httpContext = _httpContextAccessor.HttpContext;

            var photoreturnDTO = obj.Select(photo => new PhotoDTO(

                Id: photo.Id,
                Title: photo.Title,
                UploadedBy: photo.UploadedBy,
                UploadedAt: TimeZoneInfo.ConvertTimeFromUtc(photo.UploadedAt, localTimeZone),
                ThumbPath: photo.ThumbPath != null && System.IO.File.Exists(photo.ThumbPath)
                    ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/Storage/{photo.ApplicationId}/thumbnails/{Path.GetFileName(photo.ThumbPath)}"
                    : null


                )).ToList();


            return photoreturnDTO;
        }
    }

    public interface IAllListService
    {
        List<PhotoDTO> AllList();
    }

}
