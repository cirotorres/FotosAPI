using FotosAPI.Controllers;
using FotosAPI.Data;
using FotosAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;


namespace FotosAPI.Services
{
    public class DeleteObjService : IDeleteObjService
    {
        private readonly IPhotoRepository _photoRepository;

        public DeleteObjService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public Photo? FindObjById(int id)
        {
            // Busca o objeto no repositório
            return _photoRepository.Get(id);
        }

        public bool DeleteObj(int id)
        {
            // Busca o objeto no repositório
            var photoremove =  _photoRepository.Get(id);
            if (photoremove == null) throw new KeyNotFoundException("O objeto solicitado para exclusão não foi encontrado.");

            // Verifica se o ApplicationId está nulo ou vazio
            if (string.IsNullOrEmpty(photoremove.ApplicationId))
            {
                _photoRepository.Delete(id);
                return true;
            }

            // Obtém o caminho da pasta do applicationId
            var applicationDirectory = Path.Combine("Storage", photoremove.ApplicationId);

            // Realiza a exclusão dos arquivos e diretórios
            if (System.IO.File.Exists(photoremove.PicturePath))
                System.IO.File.Delete(photoremove.PicturePath);

            var thumbnailPath = Path.Combine(applicationDirectory, "thumbnails", Path.GetFileName(photoremove.PicturePath));
            if (System.IO.File.Exists(thumbnailPath))
                System.IO.File.Delete(thumbnailPath);

            if (!Directory.EnumerateFileSystemEntries(applicationDirectory).Any())
                Directory.Delete(applicationDirectory, true);

            // Exclui o objeto do banco de dados
            _photoRepository.Delete(id);

            return true;
        }
    }
}
