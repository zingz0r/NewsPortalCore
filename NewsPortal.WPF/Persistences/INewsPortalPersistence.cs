using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.Data.DTO;

namespace NewsPortal.WPF.Persistences
{
    public interface INewsPortalPersistence
    {
        Task<IEnumerable<ArticleDTO>> ReadArticlesAsync();
        Task<bool> CreateArticleAsync(ArticleDTO article);
        Task<bool> UpdateArticleAsync(ArticleDTO article);
        Task<bool> DeleteArticleAsync(ArticleDTO article);
        Task<bool> LoginAsync(String userName, String userPassword);
        Task<bool> LogoutAsync();
    }
}
