using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace FotosAPI.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
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
    }
}

