using System.Collections.Generic;
using System.Threading.Tasks;
using NewsPortal.Data.DTO;
using NewsPortal.Data.Entity;

namespace NewsPortal.WPF.Persistences
{
    public interface INewsPortalPersistence
    {
        Task<IEnumerable<ArticleDTO>> ReadArticlesAsync();
        Task<bool> CreateArticleAsync(ArticleDTO article);
        Task<bool> UpdateArticleAsync(ArticleDTO article);
        Task<bool> DeleteArticleAsync(ArticleDTO article);
        Task<User> GetLoggedInUserInfo();
        Task<bool> LoginAsync(string userName, string userPassword);
        Task<bool> LogoutAsync();
    }
}
