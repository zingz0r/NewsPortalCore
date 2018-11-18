using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NewsPortal.WPF.ViewModels.BaseViewModel;
using NewsPortal.Data.Entity;
using NewsPortal.WPF.Models;
using NewsPortal.WPF.Persistences;
using NewsPortal.WPF.Resources;
using NewsPortal.WPF.ViewModels.EventArgumentums;

namespace NewsPortal.WPF.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private readonly INewsPortalModel _model;
        private ObservableCollection<Article> _articles;
        private bool _isLoaded;
        private Article _selectedArticle;


        public event EventHandler ArticleEditingStarted;
        public event EventHandler ArticleEditingFinished;
        public event EventHandler ExitApplication;
        public event EventHandler LogoutApplication;

        public DelegateCommand SaveArticleChangesCommand { get; private set; }
        public DelegateCommand CancelArticleChangesCommand { get; private set; }


        public DelegateCommand CreateArticleCommand { get; private set; }
        public DelegateCommand UpdateArticleCommand { get; set; }
        public DelegateCommand DeleteArticleCommand { get; set; }

        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand LogoutCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }

        public ObservableCollection<Article> Articles
        {
            get => _articles;
            private set
            {
                if (_articles != value)
                {
                    _articles = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsLoaded
        {
            get { return _isLoaded; }
            private set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    OnPropertyChanged();
                }
            }
        }
        public Article EditedArticle { get; private set; }
        public Article SelectedArticle
        {
            get { return _selectedArticle; }
            set
            {
                if (_selectedArticle != value)
                {
                    _selectedArticle = value;
                    OnPropertyChanged();
                }
            }
        }

        public object NewsPortalLogo => ResourceManager.GetPngImage(Images.logo);
        public object ExitIcon => ResourceManager.GetPngImage(Images.icon_exit);
        public object LogoutIcon => ResourceManager.GetPngImage(Images.icon_logout);
        public object DownloadIcon => ResourceManager.GetPngImage(Images.icon_download);
        public object UploadIcon => ResourceManager.GetPngImage(Images.icon_upload);
        public object AddArticleIcon => ResourceManager.GetPngImage(Images.icon_addarticle);
        public object EditorIcon => ResourceManager.GetPngImage(Images.icon_edit);


        public MainViewModel(INewsPortalModel model)
        {
            _model = model ?? throw new ArgumentNullException("model");

            _model.ArticleChanged += Model_ArticleChanged;
            _isLoaded = false;

            CreateArticleCommand = new DelegateCommand(param =>
            {
                EditedArticle = new Article();  // a szerkesztett épület új lesz
                OnArticleEditingStarted();
            });

            SaveArticleChangesCommand = new DelegateCommand(param => SaveArticleChanges());
            CancelArticleChangesCommand = new DelegateCommand(param => CancelArticleChanges());

            UpdateArticleCommand = new DelegateCommand(param => UpdateArticle(param as Article));
            DeleteArticleCommand = new DelegateCommand(param => DeleteArticle(param as Article));

            LoadCommand = new DelegateCommand(param => LoadAsync());
            SaveCommand = new DelegateCommand(param => SaveAsync());
            ExitCommand = new DelegateCommand(param => OnExitApplication());
            LogoutCommand = new DelegateCommand(param => OnLogoutApplication());

            _model.ArticleChanged += Model_ArticleChanged;


            // Load data automatically
            LoadAsync();
        }

        private void SaveArticleChanges()
        {
            // validations
            if (String.IsNullOrEmpty(EditedArticle.Title))
            {
                OnMessageApplication("Title can't be empty!");
                return;
            }

            if (String.IsNullOrEmpty(EditedArticle.Summary))
            {
                OnMessageApplication("Summary can't be empty!");
                return;
            }

            if (String.IsNullOrEmpty(EditedArticle.Text))
            {
                OnMessageApplication("Text can't be empty!");
                return;
            }

            // save
            if (EditedArticle.Id == 0) // create new
            {
                _model.CreateArticle(EditedArticle);
                Articles.Add(EditedArticle);
                SelectedArticle = EditedArticle;
            }
            else // if already exists
            {
                _model.UpdateArticle(EditedArticle);
            }

            EditedArticle = null;

            OnArticleEditingFinished();
        }

        private void CancelArticleChanges()
        {
            EditedArticle = null;
            OnArticleEditingFinished();
        }

        private void DeleteArticle(Article article)
        {
            if (article == null)
                return;

            ConfirmationMessageEventArgs confirmationMessage = new ConfirmationMessageEventArgs("Are you sure, you want to delete this article?");
            OnConfirmationMessageApplication(confirmationMessage);

            if (confirmationMessage.Cancel)
                return;

            Articles.Remove(article);

            _model.DeleteArticle(article);
        }

        private void UpdateArticle(Article article)
        {
            if (article == null)
                return;

            EditedArticle = new Article
            {
                Id = article.Id,
                Author = article.Author,
                Date = article.Date,
                IsFeatured = article.IsFeatured,
                Summary = article.Summary,
                Text = article.Text,
                Title = article.Title,
                UserId = article.UserId
            };

            OnArticleEditingStarted();
        }

        private async void LoadAsync()
        {
            try
            {
                await _model.LoadAsync();
                Articles = new ObservableCollection<Article>(_model.Articles);
                IsLoaded = true;
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("Can't load data! No connection to the api.");
            }
        }

        private async void SaveAsync()
        {
            try
            {
                await _model.SaveAsync();
                OnMessageApplication("Successfully saved!");
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("Can't save data! No connection to the api.");
            }
        }

        private void Model_ArticleChanged(object sender, ArticleChangedEventArgs e)
        {
            int index = Articles.IndexOf(Articles.FirstOrDefault(x => x.Id == e.ArticleId));
            Articles.RemoveAt(index);
            Articles.Insert(index, _model.Articles[index]);

            SelectedArticle = Articles[index];
        }

        private void OnExitApplication()
        {
            ExitApplication?.Invoke(this, EventArgs.Empty);
        }

        private void OnLogoutApplication()
        {
            LogoutApplication?.Invoke(this, EventArgs.Empty);
        }

        private void OnArticleEditingStarted()
        {
            ArticleEditingStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnArticleEditingFinished()
        {
            ArticleEditingFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
