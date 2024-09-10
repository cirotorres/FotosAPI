namespace FotosAPI.DTOs
{
    public class PhotoDTO
    {
        public IFormFile? Picture { get; set; } // Upload foto + path
        public int Width { get; set; } // Largura
        public int Height { get; set; } // Altura
        public string? Title { get; set; } // Título
        public string? FilePath { get; set; } // Caminho físico
        public DateTime UploadedAt { get; set; } // Data de cadastro
        public string? UploadedBy { get; set; } // Identificação do cadastrador
        public string? ApplicationId { get; set; } // ID da aplicação
        

    }
}
