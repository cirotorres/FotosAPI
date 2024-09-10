using FotosAPI.Models;
using FotosAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FotosAPI.Services
{
    public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly string _secretKey = configuration["Jwt:SecretKey"];
    private readonly string _issuer = configuration["Jwt:Issuer"];
    private readonly string _audience = configuration["Jwt:Audience"];

        // Método 'Generate' que utiliza o mesmo formato anterior
        public string Generate(string userName, string appId)
    {
        // Cria a instância do JwtSecurityTokenHandler
        var handler = new JwtSecurityTokenHandler();

        // Cria a chave e as credenciais de assinatura usando a chave secreta
        var key = Encoding.UTF8.GetBytes(_secretKey);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        // Define as claims que serão incluídas no token
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, userName), // Nome do usuário
                new Claim("appId", appId), // ID do aplicativo
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Identificador único para o token
            };

        // Descrição do token (contendo as credenciais e claims)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1), // Expira em 1 hora
            SigningCredentials = credentials,
            Issuer = _issuer,  // Adiciona o Issuer
            Audience = _audience  // Adiciona o Audience
        };

        // Gera o token
        var token = handler.CreateToken(tokenDescriptor);

        // Retorna o token como string
        return handler.WriteToken(token);
    }
}
}
