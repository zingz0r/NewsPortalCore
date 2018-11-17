using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsPortal.Data.Entity;
using NewsPortal.WPF.Persistences;
using NewsPortal.WPF.ViewModels.EventArgumentums;

namespace NewsPortal.WPF.Models
{
    class NewsPortalModel : INewsPortalModel
    {
        private readonly INewsPortalPersistence _persistence;
        private List<Article> _articles;
        private Dictionary<Article, DataFlag> _articleFlags;

        private enum DataFlag
        {
            Create,
            Update,
            Delete
        }

        public void DeleteArticle(Article article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            // keresés azonosító alapján
            Article articleToDelete = _articles.FirstOrDefault(b => b.Id == article.Id);

            if (articleToDelete == null)
                throw new ArgumentException("The article does not exist.", nameof(article));

            // külön kezeljük, ha egy adat újonnan hozzávett (ekkor nem kell törölni a szerverről)
            if (_articleFlags.ContainsKey(articleToDelete) && _articleFlags[articleToDelete] == DataFlag.Create)
                _articleFlags.Remove(articleToDelete);
            else
                _articleFlags[articleToDelete] = DataFlag.Delete;

            _articles.Remove(articleToDelete);
        }

        public IReadOnlyList<Article> Articles => _articles;

        public async void CreateArticle(Article article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));
            if (_articles.Contains(article))
                throw new ArgumentException("The article is already in the collection.", nameof(article));

            var userInfo = (await _persistence.GetLoggedInUserInfo());
            

            // this is bullshit but in travelagency it is like this
            // if you delete the last article next create will get the same id and will not able to insert it into sql
            // article.Id = (_articles.Count > 0 ? _articles.Max(x => x.Id) : 0) + 1;

            article.Author = userInfo;
            article.UserId = userInfo.Id;
            article.Date = DateTime.Now;

            _articleFlags.Add(article, DataFlag.Create);
            _articles.Add(article);

            OnArticleChanged(article.Id);
        }

        public void UpdateArticle(Article article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));
            
            Article articleToModify = _articles.FirstOrDefault(x => x.Id == article.Id);

            if (articleToModify == null)
                throw new ArgumentException("The article does not exist.", nameof(article));

            articleToModify.Title = article.Title;
            articleToModify.Summary = article.Summary;
            articleToModify.Text = article.Text;
            articleToModify.Date = DateTime.Now;
            articleToModify.IsFeatured = article.IsFeatured;


            if (_articleFlags.ContainsKey(articleToModify) && _articleFlags[articleToModify] == DataFlag.Create)
            {
                _articleFlags[articleToModify] = DataFlag.Create;
            }
            else
            {
                _articleFlags[articleToModify] = DataFlag.Update;
            }
            
            OnArticleChanged(article.Id);
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
            _articleFlags = new Dictionary<Article, DataFlag>();
        }

        public async Task SaveAsync()
        {
            List<Article> articlesToSave = _articleFlags.Keys.ToList();

            foreach (Article article in articlesToSave)
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

        public async Task<bool> LoginAsync(string userName, string userPassword)
        {
            IsUserLoggedIn = await _persistence.LoginAsync(userName, userPassword);
            return IsUserLoggedIn;
        }

        public async Task<bool> LogoutAsync()
        {
            if (!IsUserLoggedIn)
                return true;

            IsUserLoggedIn = !(await _persistence.LogoutAsync());

            return IsUserLoggedIn;
        }

        private void OnArticleChanged(int articleId)
        {
            ArticleChanged?.Invoke(this, new ArticleChangedEventArgs { ArticleId = articleId });
        }
    }
}
