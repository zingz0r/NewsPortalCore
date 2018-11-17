using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.Data.Entity;

namespace NewsPortal.WPF.Persistences
{
    public interface INewsPortalPersistence
    {
        Task<IEnumerable<Article>> ReadArticlesAsync();
        Task<bool> CreateArticleAsync(Article article);
        Task<bool> UpdateArticleAsync(Article article);
        Task<bool> DeleteArticleAsync(Article article);
        Task<User> GetLoggedInUserInfo();
        Task<bool> LoginAsync(string userName, string userPassword);
        Task<bool> LogoutAsync();
    }
}
