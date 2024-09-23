
using FotosAPI.Models;
using FotosAPI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FotosAPI.Controllers
{
    [ApiController]
    [Route("/photos")]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly ILogger<PhotoController> _logger;
        public PhotoController(IPhotoRepository photoRepository, ILogger<PhotoController> logger)
        {
            _photoRepository = photoRepository ?? throw new ArgumentNullException(nameof(photoRepository));
            _logger = logger;

        }


        [Authorize]
        [HttpPost]
        [Route("upload")]
        public IActionResult Add([FromForm] PhotoViewModel photoView)
        {

            // Extrai as claims do token JWT
            var uploadedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Pegando "nameidentifier" (sub)
            var applicationId = User.FindFirst("appId")?.Value; // Pegando "appId"


            // Se for nulo ou vazio ele retorna não autorizado
            if (string.IsNullOrEmpty(uploadedBy) || string.IsNullOrEmpty(applicationId))
            {
                return Unauthorized("Token inválido ou informações faltando.");
            }

            // Processo de Upload Foto e suas características
            // caminho para a pasta de armazenamento da foto original com base no applicationId
            var applicationDirectory = Path.Combine("Storage", applicationId);
            if (!Directory.Exists(applicationDirectory))
            {
                Directory.CreateDirectory(applicationDirectory);
            }
            var filePath = Path.Combine(applicationDirectory, photoView.Picture.FileName);

            // Processo de redimensionamento da imagem
            int width, height;
            try
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    // Load imagem usando ImageSharp
                    using (var image = Image.Load(photoView.Picture.OpenReadStream()))
                    {
                        // Captura a largura e altura da imagem
                        width = image.Width;
                        height = image.Height;

                        // Usuário reduz a qualidade da imagem usando o atributo "Quality"
                        var encoder = new JpegEncoder { Quality = photoView.Quality };
                        image.Save(fileStream, encoder);

                        // Criando miniatura/thumbnails
                        if (photoView.Thumbnail)
                        {
                            var thumbnailDirectory = Path.Combine("Storage", applicationId, "thumbnails");
                            var thumbnailPath = Path.Combine(thumbnailDirectory, photoView.Picture.FileName);

                            if (!Directory.Exists(thumbnailDirectory))
                            {
                                Directory.CreateDirectory(thumbnailDirectory);
                            }

                            // Redimensiona a thumbnail
                            int thumbnailWidth = 150;
                            int thumbnailHeight = (image.Height * thumbnailWidth) / image.Width; // Proporção 

                            image.Mutate(x => x.Resize(thumbnailWidth, thumbnailHeight));

                            // Salva a thumbnail
                            using (var thumbnailStream = new FileStream(thumbnailPath, FileMode.Create))
                            {
                                image.Save(thumbnailStream, encoder); // Usa o mesmo encoder para a thumbnail
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao fazer upload: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a imagem.");
            }


            //Hora local
            var UploadedAt = DateTime.UtcNow;

            //Verificação Thumb
            var isthumb = photoView.Thumbnail;

            // Cria o objeto Photo com suas atribuições
            var photo = new Photo(filePath, photoView.Title, photoView.Thumbnail, width, height, UploadedAt, uploadedBy, applicationId, isthumb);

            // Adiciona a foto ao repositório
            _photoRepository.Add(photo);

            //Response com o horário local
            var localTimeZone = TimeZoneInfo.Local;
            photo.UploadedAt = TimeZoneInfo.ConvertTimeFromUtc(photo.UploadedAt, localTimeZone);

            var objeto = _photoRepository.Get();
            return Ok(photo);
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("AllList")]
        public IActionResult GetAll()
        {
            // Lista todos os objetos cadastrados
            var obj = _photoRepository.GetAll();

            if (obj == null || !obj.Any())
                return NotFound("Lista vazia!");

            var photoreturnDTO = obj.Select(photo => new PhotoDTO(

                Id: photo.Id,
                Title: photo.Title,
                UploadedBy: photo.UploadedBy,
                ApplicationId: photo.ApplicationId,
                UploadedAt: photo.UploadedAt

                )).ToList();


            return Ok(photoreturnDTO);
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("find/{id}")]
        public IActionResult Get(int id)
        {
            // Encontra o objeto cadastrado
            var obj = _photoRepository.Get(id);
            if (obj == null)
                return NotFound("Objeto não encontrado");


            return Ok(obj);
        }



        [HttpGet]
        [Route("view/{id}")]
        [ResponseCache(Duration = 300)]
        public IActionResult DownloadPhoto(int id)
        {
            var pic = _photoRepository.Get(id);
            if (pic == null)
                return NotFound("Foto não encontrada");

            var dataBytes = System.IO.File.ReadAllBytes(pic.PicturePath);

            using (var image = Image.Load(pic.PicturePath))
            {
                // Adiciona informações de largura, altura e título nos headers
                Response.Headers.Add("Photo-Width", image.Width.ToString());
                Response.Headers.Add("Photo-Height", image.Height.ToString());
                Response.Headers.Add("Photo-Title", pic.Title);
                Response.Headers.Add("Photo-CreationDate", pic.UploadedAt.ToString("o"));
            }

            return File(dataBytes, "image/jpg");
        }


        [Authorize]
        [HttpDelete]
        [Route("delete/{id}")]

        public IActionResult Delete(int id)
        {
            // Busca o objeto no repositório
            var photoremove = _photoRepository.Get(id);

            if (photoremove == null)
            {
                // Retorna 404 objeto não encontrado
                return NotFound("Objeto não encontrado");
            }

            // Verifica se o ApplicationId está nulo ou vazio
            if (string.IsNullOrEmpty(photoremove.ApplicationId))
            {
                // Deleta o objeto do banco de dados
                _photoRepository.Delete(id);

                return Ok(new { message = "Objeto foi deletado, mas não havia fotos relacionadas." });
            }

            // Obtém o caminho da pasta do applicationId
            var applicationDirectory = Path.Combine("Storage", photoremove.ApplicationId);

            try
            {
                // Verifica se a foto original existe e deleta
                if (System.IO.File.Exists(photoremove.PicturePath))
                {
                    System.IO.File.Delete(photoremove.PicturePath);
                }

                // Verifica se o arquivo da thumbnail existe e deleta
                var thumbnailPath = Path.Combine(applicationDirectory, "thumbnails", Path.GetFileName(photoremove.PicturePath));
                if (System.IO.File.Exists(thumbnailPath))
                {
                    System.IO.File.Delete(thumbnailPath);
                }

                // Verifica se o diretório está vazio após deletar as fotos
                if (!Directory.EnumerateFileSystemEntries(applicationDirectory).Any())
                {
                    // Se estiver vazio, deleta o diretório
                    Directory.Delete(applicationDirectory, true);
                }
            }
            catch (Exception ex)
            {
                // Se houver erro, retorna 400 com a mensagem de erro
                return StatusCode(400, $"Erro ao deletar: {ex.Message}");
            }

            // Deleta o objeto do banco de dados
            _photoRepository.Delete(id);

            // Retorna 204 No Content para indicar que a operação foi bem-sucedida
            return Ok(new { message = "Objeto, Foto e miniatura deletados com sucesso." });
        }



    }
}