using FotosAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FotosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        // Injeta o TokenService no AuthController
        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        
        [HttpGet("generate-token")]
        public IActionResult GenerateToken()
        {
            // Dados do usuário e aplicativo 
            string userName = "Ciro Torres DEV";
            string appId = "APIPhototeste";

            // Gera o token
            var token = _tokenService.Generate(userName, appId);

            // Retorna o token em um JSON
            return Ok(new { Token = token });
        }
    }
}


