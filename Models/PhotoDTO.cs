namespace FotosAPI.Models
{
    public record PhotoDTO(int Id, string? Title, string? UploadedBy, string? ApplicationId, DateTime UploadedAt);

}
