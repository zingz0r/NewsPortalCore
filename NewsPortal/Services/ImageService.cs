using System.Collections.Generic;
using System.Linq;
using NewsPortal.Data.Entity;
using NewsPortal.Interfaces;

namespace NewsPortal.Services
{
    public class ImageService : IImageService
    {
        private readonly NewsPortalContext _context;

        public ImageService(NewsPortalContext context)
        {
            _context = context;
        }
        public List<int> GetPictureIdsForAnArticle(int? articleId)
        {
            if (articleId == null)
            {
                return new List<int>();
            }
            
            return _context.Picture.Where(x => x.ArticleId == articleId).Select(x => x.Id).ToList();
        }
    }
}
