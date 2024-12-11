namespace FotosAPI.Models
{
    public record PhotoDTO(int Id, string? Title, string? UploadedBy, DateTime UploadedAt, string? ThumbPath);

}
