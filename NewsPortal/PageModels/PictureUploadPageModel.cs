using Microsoft.AspNetCore.Http;
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
