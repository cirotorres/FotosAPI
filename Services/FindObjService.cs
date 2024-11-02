using FotosAPI.Data;
using FotosAPI.Models;

namespace FotosAPI.Services
{
    public class FindObjService : IFindObjService
    {
        private readonly IPhotoRepository _photoRepository;

        public FindObjService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public Photo FindObj(int id)
        {
            // Encontra o objeto cadastrado
            var obj = _photoRepository.Get(id);
            if (obj == null)
                throw new KeyNotFoundException("Objeto não encontrado.");

            // Ajuste horário local no response body
            var localTimeZone = TimeZoneInfo.Local;
            obj.UploadedAt = TimeZoneInfo.ConvertTimeFromUtc(obj.UploadedAt, localTimeZone);

            return obj;
        }
    }

    public interface IFindObjService
    {
        Photo FindObj(int id);
    }
}
