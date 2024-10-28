
using FotosAPI.Models;
using FotosAPI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp;
using System.Security.Claims;
using FotosAPI.Services;

namespace FotosAPI.Controllers
{
    [ApiController]
    [Route("/photos")]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly ILogger<PhotoController> _logger;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IDeleteObjService _deleteObjService;
        public PhotoController(IPhotoRepository photoRepository, ILogger<PhotoController> logger, IImageProcessingService imageProcessingService, IDeleteObjService deleteObjService)
        {
            _photoRepository = photoRepository ?? throw new ArgumentNullException(nameof(photoRepository));
            _logger = logger;
            _imageProcessingService = imageProcessingService;
            _deleteObjService = deleteObjService;
        }


        [Authorize]
        [HttpPost]
        [Route("upload")]
        public IActionResult Add([FromForm] PhotoViewModel photoView)
        {
            // Extrai as claims do token JWT
            var uploadedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var applicationId = User.FindFirst("appId")?.Value;

            // Se for nulo ou vazio ele retorna não autorizado
            if (string.IsNullOrEmpty(uploadedBy) || string.IsNullOrEmpty(applicationId))
            {
                return Unauthorized("Token inválido ou informações faltando.");
            }

            try
            {
                // Chama o serviço/método de processamento completo de imagem
                var photo = _imageProcessingService.AllImageProcess(photoView, uploadedBy, applicationId);

                return Ok(photo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao fazer upload: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a imagem.");
            }



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

            var localTimeZone = TimeZoneInfo.Local;

            var photoreturnDTO = obj.Select(photo => new PhotoDTO(

                Id: photo.Id,
                Title: photo.Title,
                UploadedBy: photo.UploadedBy,
                ApplicationId: photo.ApplicationId,
                UploadedAt: TimeZoneInfo.ConvertTimeFromUtc(photo.UploadedAt, localTimeZone)

                )).ToList();


            return Ok(photoreturnDTO);
        }


        [Authorize]
        [HttpGet]
        [Route("find/{id}")]
        public IActionResult Get(int id)
        {
            
            // Encontra o objeto cadastrado
            var obj = _photoRepository.Get(id);
            if (obj == null)
                return NotFound("Objeto não encontrado");

            //Ajuste horário local no response body
            var localTimeZone = TimeZoneInfo.Local;
            obj.UploadedAt = TimeZoneInfo.ConvertTimeFromUtc(obj.UploadedAt, localTimeZone);

            return Ok(obj);
        }

        [Authorize]
        [HttpGet]
        [Route("view/{id}")]
        [ResponseCache(Duration = 300)]
        public IActionResult DownloadPhoto(int id)
        {
            var pic = _photoRepository.Get(id);
            if (pic == null)
                return NotFound("Foto não encontrada");

            var dataBytes = System.IO.File.ReadAllBytes(pic.PicturePath);

            //Ajuste horário local no response body
            var localTimeZone = TimeZoneInfo.Local;
            pic.UploadedAt = TimeZoneInfo.ConvertTimeFromUtc(pic.UploadedAt, localTimeZone);

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
            try
            {
                // Chama o método de exclusão
                bool deleteSuccessful = _deleteObjService.DeleteObj(id);
             
                if (deleteSuccessful)
                    return Ok(new { message = "Objeto, Foto e miniatura deletados com sucesso." });
            }
            catch (KeyNotFoundException)
            {
                // Retorna 404 caso o objeto não seja encontrado
                return NotFound("Objeto não encontrado");
            }
            catch (IOException ex)
            {
                // Retorna erro específico de I/O
                _logger.LogError($"Erro ao excluir arquivos: {ex.Message}");
                return StatusCode(500, $"Erro ao deletar: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Captura outras exceções
                _logger.LogError($"Erro desconhecido: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a exclusão.");
            }

            return StatusCode(500, "Erro ao deletar o objeto.");
        }



    }
}
