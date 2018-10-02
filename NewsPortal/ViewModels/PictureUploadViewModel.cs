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
        public IFormFile ImageData { get; set; }
        public int ArticleId { get; set; }
    }
}
