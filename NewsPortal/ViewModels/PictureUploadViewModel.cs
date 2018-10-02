using Microsoft.AspNetCore.Http;
using NewsPortal.Models.Entiy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.ViewModels
{
    public class PictureUploadViewModel
    {
        public IFormFile LargeImageData { get; set; }
        public IFormFile SmallImageData { get; set; }
        public int ArticleId { get; set; }
    }
}
