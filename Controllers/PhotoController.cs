
using FotosAPI.Models;
using FotosAPI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp;
using System.Security.Claims;
using FotosAPI.Services;
using System;

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
        private readonly IAuthClaimsService _authClaimsService;
        private readonly IFindObjService _findObjService;
        private readonly IViewObjService _viewObjService;
        private readonly IAllListService _allListService;

        public PhotoController(IPhotoRepository photoRepository, 
            ILogger<PhotoController> logger, 
            IImageProcessingService imageProcessingService, 
            IDeleteObjService deleteObjService, 
            IAuthClaimsService authClaimsService, 
            IFindObjService findObjService, 
            IViewObjService viewObjService, 
            IAllListService allListService)
        {
            _photoRepository = photoRepository ?? throw new ArgumentNullException(nameof(photoRepository));
            _logger = logger;
            _imageProcessingService = imageProcessingService;
            _deleteObjService = deleteObjService;
            _authClaimsService = authClaimsService;
            _findObjService = findObjService;
            _viewObjService = viewObjService;
            _allListService = allListService;
        }


        [Authorize]
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Add([FromForm] PhotoViewModel photoView)
        {
           
            try
            {
                // Chama o serviço/método de extração de Claims do usuário. 
                var (uploadedBy, applicationId) = _authClaimsService.GetUserClaims();

                // Chama o serviço/método de processamento completo de imagem
                var photo = await _imageProcessingService.AllImageProcess(photoView, uploadedBy, applicationId);

                return Ok(photo);
            }
          
            catch (UnauthorizedAccessException ex)
            {
                // Retorna erro ao extrair Claims.
                _logger.LogError($"Erro na extração: {ex.Message}");
                return Unauthorized(ex.Message);
            }

            catch (UnknownImageFormatException ex)
            {
                // Retorna erro de formato.
                _logger.LogError($"Erro no formato: {ex.Message}");
                return StatusCode(500, "Formato incorreto. Formatos desejados: JPEG, JPG, PNG, BMP");
            }
  
            catch (Exception ex)
            {
                // Outros erros.
                _logger.LogError($"Erro ao fazer upload: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar o upload. Entre em contato com o suporte.");
            }

        }


        [AllowAnonymous]
        [HttpGet]
        [Route("AllList")]
        public IActionResult GetAll()
        {
            try
            {
                // Lista todos os objetos cadastrados.
                var photos = _allListService.AllList();
                return Ok(photos);
            }

            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound("Lista vazia!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro: {ex.Message}");
                return StatusCode(500, "Erro interno . Entre em contato com o suporte.");
            }
        }


        [Authorize]
        [HttpGet]
        [Route("find/{id}")]
        public IActionResult Get(int id)
        {

            try
            {
                // Detalha o objeto selecionado.
                var obj = _findObjService.FindObj(id);
                return Ok(obj);
            }

            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound("Erro: Objeto não encontrado.");
            }

            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar objeto: {ex.Message}");
                return StatusCode(500, "Erro interno ao buscar. Entre em contato com o suporte.");
            }

        }


        [Authorize]
        [HttpGet]
        [Route("view/{id}")]
        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> DownloadPhoto(int id)
        {
            try
            {
                // Chama o serviço para obter a foto do objeto.
                var (dataBytes, pic) = await _viewObjService.ViewObj(id);

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
            catch (FileNotFoundException ex)
            {
                _logger.LogError($"Erro: {ex.Message}");
                return NotFound("Erro: Foto não encontrada.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar foto: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a solicitação. Entre em contato com o suporte.");
            }
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
                    return Ok("Objeto, Foto e miniatura deletados com sucesso.");
            }
            catch (KeyNotFoundException ex)
            {
                // Retorna objeto não encontrado.
                _logger.LogError($"Erro: {ex.Message}");
                return NotFound("Erro: O objeto não foi encontrado.");
            }

            catch (Exception ex)
            {
                // Outros erros.
                _logger.LogError($"Erro ao excluir objeto: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a exclusão. Entre em contato com o suporte.");
            }
 
            return StatusCode(500, "Erro ao deletar o objeto.");

        }

    }

}
