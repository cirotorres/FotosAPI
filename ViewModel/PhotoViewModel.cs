using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FotosAPI.ViewModel
{
    public class PhotoViewModel
    {
        [Required(ErrorMessage = "A imagem é obrigatória.")]
        public IFormFile? Picture { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
        public string Title { get; set; }

        [Range(1, 100, ErrorMessage = "A qualidade deve estar entre 1 e 100.")]
        public int Quality { get; set; }

        public bool Thumbnail { get; set; }
    }

}
