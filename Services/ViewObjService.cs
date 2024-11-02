using FotosAPI.Models;
using System.Data;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Mvc;

namespace FotosAPI.Services
{
    public class ViewObjService : IViewObjService
    {

        private readonly IPhotoRepository _photoRepository;

        public ViewObjService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }
        public (byte[] dataBytes, Photo photo) ViewObj(int id)
        {
            // Busca a foto no repositório
            var pic = _photoRepository.Get(id);
            if (pic == null)
                throw new FileNotFoundException("Foto não encontrada.");

            // Lê o conteúdo da imagem em bytes
            var dataBytes = File.ReadAllBytes(pic.PicturePath);

            // Ajuste horário local no objeto Photo
            var localTimeZone = TimeZoneInfo.Local;
            pic.UploadedAt = TimeZoneInfo.ConvertTimeFromUtc(pic.UploadedAt, localTimeZone);

            return (dataBytes, pic);
        }
    }

    public interface IViewObjService
    {
        (byte[] dataBytes, Photo photo) ViewObj(int id);
    }
}
