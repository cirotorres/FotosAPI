using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FotosAPI.Models
{
    [Table("photo")]
    public class Photo
    {
        
        [Key]

        public int Id { get; set; } // ID da foto
        public string? PicturePath { get; set; } // Foto
        public string Title { get; set; } // Título
        public int Width { get; set; } // Largura
        public int Height { get; set; } // Altura           
        public DateTime UploadedAt { get; set; } // Data de cadastro
        public string? UploadedBy { get; set; } // Identificação do cadastrador
        public string? ApplicationId { get; set; } // ID da aplicação
        [Column("thumbnail")]
        public bool? IsThumbnail { get; set; } // ID do thumbnail (caso solicitado)


        public Photo(int id, string? picture, string title, int width, int height, DateTime uploadedAt, string? uploadedBy, string? applicationId, bool? isThumbnail)
        {
            Id = id;
            PicturePath = picture;
            Title = title;
            Width = width;
            Height = height;          
            UploadedAt = uploadedAt;
            UploadedBy = uploadedBy;
            ApplicationId = applicationId; //Token
            IsThumbnail = isThumbnail;
        }

        

        //post upload imagem
        public Photo(string picture, string title, bool thumbnailId, int width, int height, DateTime uploadedAt, string? uploadedBy, string? applicationId, bool? isThumbnail)
        {
            PicturePath = picture;
            Title = title;
            Width = width;
            Height = height;
            UploadedAt = uploadedAt;
            UploadedBy = uploadedBy;
            ApplicationId = applicationId;
            IsThumbnail = isThumbnail;

        }

        public Photo() { }



    }
}
