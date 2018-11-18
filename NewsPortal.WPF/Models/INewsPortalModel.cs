using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewsPortal.Data.DTO;
using NewsPortal.WPF.ViewModels.EventArgumentums;

namespace NewsPortal.WPF.Models
{
    public interface INewsPortalModel
    {
        event EventHandler<ArticleChangedEventArgs> ArticleChanged;

        void DeleteArticle(ArticleDTO article);

        IReadOnlyList<ArticleDTO> Articles { get; }

        void CreateArticle(ArticleDTO article);
        void UpdateArticle(ArticleDTO article);

        Task LoadAsync();
        Task SaveAsync();
        Task<bool> LoginAsync(string userName, string userPassword);
        Task<bool> LogoutAsync();
    }
}
