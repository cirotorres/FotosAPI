using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using FotosAPI.Models;
using System.Security.Claims;
using FotosAPI.Data;
using FotosAPI.ViewModel;

namespace FotosAPI.Services
{
    public class ImageProcessingService : IImageProcessingService
    {

        private readonly IPhotoRepository _photoRepository;

        public ImageProcessingService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }
        public (int width, int height) ProcessAndSaveImage(Stream imageStream, string filePath, int quality)
        {
            //Load imagem usando ImageSharp
            using (var image = Image.Load(imageStream))
            {
                //Captura altura e largura
                int width = image.Width;
                int height = image.Height;

                // Salva a imagem com a qualidade especificada em "quality"
                var encoder = new JpegEncoder { Quality = quality };
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.Save(fileStream, encoder);
                }

                return (width, height);
            }
        }

        public void CreateThumbnail(string originalFilePath, string thumbnailPath, int thumbnailWidth)
        {
            using (var image = Image.Load(originalFilePath))
            {
                // Calculo para dimensionamento da thumb
                int thumbnailHeight = (image.Height * thumbnailWidth) / image.Width;

                // Redimensiona e salva a thumbnail
                image.Mutate(x => x.Resize(thumbnailWidth, thumbnailHeight));

                var encoder = new JpegEncoder { Quality = 75 }; // Define uma qualidade 
                using (var thumbnailStream = new FileStream(thumbnailPath, FileMode.Create))
                {
                    image.Save(thumbnailStream, encoder);
                }
            }
        }


        public Photo AllImageProcess(PhotoViewModel photoView, string uploadedBy, string applicationId)
        {
            // Criação do caminho para a pasta de armazenamento da foto original com base no applicationId
            var applicationDirectory = Path.Combine("Storage", applicationId);
            if (!Directory.Exists(applicationDirectory))
            {
                Directory.CreateDirectory(applicationDirectory);
            }

            var filePath = Path.Combine(applicationDirectory, photoView.Picture.FileName);

            // Chamando o serviço de processamento de imagem.
            var (width, height) = ProcessAndSaveImage(photoView.Picture.OpenReadStream(), filePath, photoView.Quality);

            if (photoView.Thumbnail)
            {
                var thumbnailDirectory = Path.Combine(applicationDirectory, "thumbnails");
                var thumbnailPath = Path.Combine(thumbnailDirectory, photoView.Picture.FileName);

                if (!Directory.Exists(thumbnailDirectory))
                {
                    Directory.CreateDirectory(thumbnailDirectory);
                }

                // Chamando o serviço de criação de thumb.
                CreateThumbnail(filePath, thumbnailPath, 150); // Thumbnail com largura fixa de 150px
            }

            var uploadedAt = DateTime.UtcNow;
            var photo = new Photo(filePath, photoView.Title, photoView.Thumbnail, width, height, uploadedAt, uploadedBy, applicationId, photoView.Thumbnail);

            _photoRepository.Add(photo);

            // Converte o horário do upload para o timezone local
            var localTimeZone = TimeZoneInfo.Local;
            photo.UploadedAt = TimeZoneInfo.ConvertTimeFromUtc(photo.UploadedAt, localTimeZone);

            return photo; // Retorna o objeto Photo atualizado
        }



    }
}

