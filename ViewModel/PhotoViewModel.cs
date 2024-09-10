using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace FotosAPI.ViewModel
{
    public class PhotoViewModel
    {
        public IFormFile? Picture { get; set; }
        public string Title { get; set; }
        public int Quality { get; set; }
        public bool Thumbnail { get; set; }
        
    }

}
