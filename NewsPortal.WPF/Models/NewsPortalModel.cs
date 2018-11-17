using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsPortal.Data.DTO;
using NewsPortal.WPF.Persistences;
using NewsPortal.WPF.ViewModels.EventArgumentums;

namespace NewsPortal.WPF.Models
{
    class NewsPortalModel : INewsPortalModel
    {
        private readonly INewsPortalPersistence _persistence;
        private List<ArticleDTO> _articles;
        private Dictionary<ArticleDTO, DataFlag> _articleFlags;

        private enum DataFlag
        {
            Create,
            Update,
            Delete
        }

        public void DeleteArticle(ArticleDTO article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            // keresés azonosító alapján
            ArticleDTO articleToDelete = _articles.FirstOrDefault(b => b.Id == article.Id);

            if (articleToDelete == null)
                throw new ArgumentException("The article does not exist.", nameof(article));

            // külön kezeljük, ha egy adat újonnan hozzávett (ekkor nem kell törölni a szerverről)
            if (_articleFlags.ContainsKey(articleToDelete) && _articleFlags[articleToDelete] == DataFlag.Create)
                _articleFlags.Remove(articleToDelete);
            else
                _articleFlags[articleToDelete] = DataFlag.Delete;

            _articles.Remove(articleToDelete);
        }

        public IReadOnlyList<ArticleDTO> Articles => _articles;

        public void CreateArticle(ArticleDTO article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));
            if (_articles.Contains(article))
                throw new ArgumentException("The article is already in the collection.", nameof(article));

            article.Id = (_articles.Count > 0 ? _articles.Max(x => x.Id) : 0) + 1;

            _articleFlags.Add(article, DataFlag.Create);
            _articles.Add(article);
        }

        public void UpdateArticle(ArticleDTO article)
        {
            throw new NotImplementedException();
        }

        public bool IsUserLoggedIn { get; private set; }

        public NewsPortalModel(INewsPortalPersistence persistence)
        {
            IsUserLoggedIn = false;
            _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
        }

        public event EventHandler<ArticleChangedEventArgs> ArticleChanged;

        public async Task LoadAsync()
        {
            _articles = (await _persistence.ReadArticlesAsync()).ToList();
            _articleFlags = new Dictionary<ArticleDTO, DataFlag>();
        }

        public async Task SaveAsync()
        {
            List<ArticleDTO> articlesToSave = _articleFlags.Keys.ToList();

            foreach (ArticleDTO article in articlesToSave)
            {
                bool result = true;
                
                switch (_articleFlags[article])
                {
                    case DataFlag.Create:
                        result = await _persistence.CreateArticleAsync(article);
                        break;
                    case DataFlag.Delete:
                        result = await _persistence.DeleteArticleAsync(article);
                        break;
                    case DataFlag.Update:
                        result = await _persistence.UpdateArticleAsync(article);
                        break;
                }

                if (!result)
                    throw new InvalidOperationException("Operation " + _articleFlags[article] + " failed on article " + article.Id);

                // ha sikeres volt a mentés, akkor törölhetjük az állapotjelzőt
                _articleFlags.Remove(article);
            }
        }

        public async Task<Boolean> LoginAsync(String userName, String userPassword)
        {
            IsUserLoggedIn = await _persistence.LoginAsync(userName, userPassword);
            return IsUserLoggedIn;
        }

        public async Task<Boolean> LogoutAsync()
        {
            if (!IsUserLoggedIn)
                return true;

            IsUserLoggedIn = !(await _persistence.LogoutAsync());

            return IsUserLoggedIn;
        }
    }
}
