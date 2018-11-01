using System.Collections.Generic;

namespace NewsPortal.Interfaces
{
    public interface IImageService
    {
        List<int> GetPictureIdsForAnArticle(int? articleId);
    }
}
