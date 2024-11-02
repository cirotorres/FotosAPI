using System.Security.Claims;

namespace FotosAPI.Services
{
   
    public class AuthClaimsService : IAuthClaimsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthClaimsService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public (string uploadedBy, string applicationId) GetUserClaims()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            // Extração das CLAIMS.
            var uploadedBy = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var applicationId = user?.FindFirst("appId")?.Value;

            if (string.IsNullOrEmpty(uploadedBy) || string.IsNullOrEmpty(applicationId))
            {
                string missingClaim = string.IsNullOrEmpty(uploadedBy) ? "uploadedBy" : "applicationId";
                throw new UnauthorizedAccessException($"Token inválido: a claim '{missingClaim}' está faltando.");
            }

            return (uploadedBy, applicationId);
        }
    }
    public interface IAuthClaimsService
    {
        (string uploadedBy, string applicationId) GetUserClaims();
    }
}
    

