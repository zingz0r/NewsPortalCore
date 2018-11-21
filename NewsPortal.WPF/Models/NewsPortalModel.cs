using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NewsPortal.Data.DTO;
using NewsPortal.Data.Entity;
using NewsPortal.WPF.Persistences;
using NewsPortal.WPF.ViewModels.EventArgumentums;

namespace NewsPortal.WPF.Models
{
    class NewsPortalModel : INewsPortalModel
    {
        private readonly INewsPortalPersistence _persistence;
        private List<ArticleDTO> _articles;

        private enum DataFlag
        {
            Create,
            Update,
            Delete
        }
        public IReadOnlyList<ArticleDTO> Articles => _articles;

        public event EventHandler<ArticleChangedEventArgs> ArticleChanged;

        public async void CreateArticle(ArticleDTO article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));
            if (_articles.Contains(article))
                throw new ArgumentException("The article is already in the collection.", nameof(article));


            // fuck this shit
            // after first await the images inside the articles will be null????????????????????
            ObservableCollection<PictureDTO> imagesCopy = new ObservableCollection<PictureDTO>();
            foreach (var image in article.Images)
            {
                imagesCopy.Add(image.Clone() as PictureDTO);
            }


            var userInfo = (await _persistence.GetLoggedInUserInfo());

            // db will generate next id
            // article.Id = (_articles.Count > 0 ? _articles.Max(x => x.Id) : 0) + 1;

            article.Author = userInfo;
            article.UserId = userInfo.Id;
            article.Date = DateTime.Now;

            if (article.Images == null)
            {
                article.Images = new ObservableCollection<PictureDTO>();
            }

            // create article
            if (await _persistence.CreateArticleAsync(article))
            {
                foreach (var image in imagesCopy)
                {
                    image.ArticleId = article.Id;
                    // create images
                    if (await _persistence.CreateArticleImageAsync(image))
                    {
                        article.Images.Add(image);
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to create image " + image.Id);
                    }
                }

                _articles.Add(article);
                OnArticleChanged(article.Id);
            }
            else
            {
                throw new InvalidOperationException("Failed to create article " + article.Id);
            }
        }

        public async void DeleteArticle(ArticleDTO article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            ArticleDTO articleToDelete = _articles.FirstOrDefault(b => b.Id == article.Id);

            if (articleToDelete == null)
                throw new ArgumentException("The article does not exist.", nameof(article));

            if (await _persistence.DeleteArticleAsync(article))
            {

            }

            _articles.Remove(articleToDelete);
        }

        public async void UpdateArticle(ArticleDTO editArticle)
        {
            if (editArticle == null)
                throw new ArgumentNullException(nameof(editArticle));

            ArticleDTO articleToModify = _articles.FirstOrDefault(x => x.Id == editArticle.Id);

            if (articleToModify == null)
                throw new ArgumentException("The article does not exist.", nameof(editArticle));

            articleToModify.Title = editArticle.Title;
            articleToModify.Summary = editArticle.Summary;
            articleToModify.Text = editArticle.Text;
            articleToModify.Date = DateTime.Now;
            articleToModify.IsFeatured = editArticle.IsFeatured;

            // fuck this shit
            // after first await the images inside the articles will be null????????????????????
            ObservableCollection<PictureDTO> imagesCopy = new ObservableCollection<PictureDTO>();
            foreach (var image in editArticle.Images)
            {
                imagesCopy.Add(image.Clone() as PictureDTO);
            }

            // remove deleted images
            Dictionary<PictureDTO, DataFlag> deleteImageFlags = new Dictionary<PictureDTO, DataFlag>();
            foreach (var image in articleToModify.Images)
            {
                if (!imagesCopy.Contains(image))
                {
                    if (await _persistence.DeleteArticleImageAsync(image))
                    {
                        deleteImageFlags.Add(image, DataFlag.Delete);
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to create image " + image.Id);
                    }
                }
            }

            foreach (var flag in deleteImageFlags)
                if (flag.Value == DataFlag.Delete)
                    articleToModify.Images.Remove(flag.Key);

            // add new images
            Dictionary<PictureDTO, DataFlag> addImageFlags = new Dictionary<PictureDTO, DataFlag>();
            foreach (var image in imagesCopy)
            {
                if (!articleToModify.Images.Contains(image))
                {
                    image.ArticleId = articleToModify.Id;
                    if (await _persistence.CreateArticleImageAsync(image))
                    {
                        addImageFlags.Add(image, DataFlag.Create);
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to create image " + image.Id);
                    }
                }
            }
            foreach (var flag in addImageFlags)
                if (flag.Value == DataFlag.Create)
                    articleToModify.Images.Add(flag.Key);


            if (await _persistence.UpdateArticleAsync(articleToModify))
            {

            }
            else
            {
                throw new InvalidOperationException("Failed to update article " + articleToModify.Id);

            }

            OnArticleChanged(articleToModify.Id);
        }

        public bool IsUserLoggedIn { get; private set; }

        public NewsPortalModel(INewsPortalPersistence persistence)
        {
            IsUserLoggedIn = false;
            _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
        }

        public async Task LoadAsync()
        {
            _articles = (await _persistence.ReadArticlesAsync()).ToList();
        }

        public async Task<bool> LoginAsync(string userName, string userPassword)
        {
            IsUserLoggedIn = await _persistence.LoginAsync(userName, userPassword);
            return IsUserLoggedIn;
        }

        public async Task<bool> LogoutAsync()
        {
            _articles.Clear();

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
