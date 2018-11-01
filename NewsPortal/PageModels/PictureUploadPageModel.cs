using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NewsPortal.PageModels
{
    public class PictureUploadPageModel : PageModel
    {
        public IFormFile LargeImageData { get; set; }
        public IFormFile SmallImageData { get; set; }
        public int ArticleId { get; set; }
    }
}
