using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.Data.Entity;

namespace NewsPortal.Controllers
{
    public class ManagerController : Controller
    {
        private readonly NewsPortalContext _context;

        public ManagerController(NewsPortalContext context)
        {
            _context = context;
        }

        public FileResult GetImage(int? id, bool isLarge = false)
        {
            if (id == null)
                return File("~/images/NoImage.png", "image/png");

            byte[] imageContent = _context.Picture
                .Where(image => image.Id == id)
                    .Select(image => isLarge ? image.LargeImageData : image.SmallImageData)
                        .FirstOrDefault();

            if (imageContent == null)
                return File("~/images/NoImage.png", "image/png");

            return File(imageContent, "image/png");
        }

        public FileResult GetFirstSmallImageForNewsId(int? articleId)
        {
            if (articleId == null)
                return File("~/images/NoImage.png", "image/png");

            byte[] imageContent = _context.Picture
                .Where(x => x.ArticleId == articleId)
                    .Select(x => x.SmallImageData)
                        .FirstOrDefault();

            if (imageContent == null)
                return File("~/images/NoImage.png", "image/png");

            return File(imageContent, "image/png");
        }
    }
}