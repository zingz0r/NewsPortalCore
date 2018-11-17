using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewsPortal.Data.Entity;
using NewsPortal.WPF.ViewModels.EventArgumentums;

namespace NewsPortal.WPF.Models
{
    public interface INewsPortalModel
    {
        event EventHandler<ArticleChangedEventArgs> ArticleChanged;

        void DeleteArticle(Article article);

        IReadOnlyList<Article> Articles { get; }

        void CreateArticle(Article article);
        void UpdateArticle(Article article);

        Task LoadAsync();
        Task SaveAsync();
        Task<bool> LoginAsync(string userName, string userPassword);
        Task<bool> LogoutAsync();
    }
}
