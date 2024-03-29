﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using NewsPortal.Data.DTO;
using NewsPortal.WPF.ViewModels.BaseViewModel;
using NewsPortal.WPF.Models;
using NewsPortal.WPF.Persistences;
using NewsPortal.WPF.Resources;
using NewsPortal.WPF.ViewModels.EventArgumentums;

namespace NewsPortal.WPF.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private readonly INewsPortalModel _model;

        private ObservableCollection<ArticleDTO> _articles;

        private bool _isLoaded;
        private ArticleDTO _selectedArticle;

        public event EventHandler ArticleEditingStarted;
        public event EventHandler ArticleEditingFinished;
        public event EventHandler ExitApplication;
        public event EventHandler LogoutApplication;
        public event EventHandler<ImageEventArgs> ImageEditingStarted;

        public DelegateCommand SaveArticleChangesCommand { get; private set; }
        public DelegateCommand CancelArticleChangesCommand { get; private set; }

        public DelegateCommand CreateArticleCommand { get; private set; }
        public DelegateCommand UpdateArticleCommand { get; set; }
        public DelegateCommand DeleteArticleCommand { get; set; }

        public DelegateCommand CreateImageCommand { get; set; }
        public DelegateCommand DeleteImageCommand { get; set; }

        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand LogoutCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }

        public ObservableCollection<ArticleDTO> Articles
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
        public ArticleDTO EditedArticle { get; private set; }
        public ArticleDTO SelectedArticle
        {
            get => _selectedArticle;
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
                EditedArticle = new ArticleDTO();

                OnArticleEditingStarted();
            });

            SaveArticleChangesCommand = new DelegateCommand(param => SaveArticleChanges());
            CancelArticleChangesCommand = new DelegateCommand(param => CancelArticleChanges());

            UpdateArticleCommand = new DelegateCommand(param => EditArticle(param as ArticleDTO));
            DeleteArticleCommand = new DelegateCommand(param => DeleteArticle(param as ArticleDTO));

            CreateImageCommand = new DelegateCommand(param => OnImageEditingStarted((param as ArticleDTO).Id));
            DeleteImageCommand = new DelegateCommand(param => DeleteImage(param as PictureDTO));

            LoadCommand = new DelegateCommand(param => LoadAsync());
            ExitCommand = new DelegateCommand(param => OnExitApplication());
            LogoutCommand = new DelegateCommand(param => OnLogoutApplication());

            _model.ArticleChanged += Model_ArticleChanged;

            // Load data automatically
            LoadAsync();
        }

        private void SaveArticleChanges()
        {
            // validations
            if (string.IsNullOrEmpty(EditedArticle.Title))
            {
                OnMessageApplication("Title can't be empty!");
                return;
            }

            if (string.IsNullOrEmpty(EditedArticle.Summary))
            {
                OnMessageApplication("Summary can't be empty!");
                return;
            }

            if (string.IsNullOrEmpty(EditedArticle.Text))
            {
                OnMessageApplication("Text can't be empty!");
                return;
            }

            if (EditedArticle.IsFeatured && EditedArticle.Images.Count == 0)
            {
                OnMessageApplication("At least one image has to be uploaded!");
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

            EditedArticle.Images = null;
            EditedArticle = null;

            OnArticleEditingFinished();
        }

        private void CancelArticleChanges()
        {
            //EditedArticle.Images = null;
            EditedArticle = null;

            OnArticleEditingFinished();
        }

        private void DeleteArticle(ArticleDTO article)
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

        private void EditArticle(ArticleDTO article)
        {
            if (article == null)
                return;

            EditedArticle = new ArticleDTO(article.Clone() as ArticleDTO);

            OnArticleEditingStarted();
        }

        private async void LoadAsync()
        {
            try
            {
                await _model.LoadAsync();
                Articles = new ObservableCollection<ArticleDTO>(_model.Articles);
                IsLoaded = true;
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("Can't load data! No connection to the api.");
            }
        }

        private void Model_ArticleChanged(object sender, ArticleChangedEventArgs e)
        {
            int index = Articles.IndexOf(Articles.FirstOrDefault(x => x.Id == e.ArticleId));
            Articles.RemoveAt(index);
            Articles.Insert(index, _model.Articles[index]);

            SelectedArticle = Articles[index];
            EditedArticle = Articles[index];
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

        private void OnImageEditingStarted(int articleId)
        {
            var imageEventArgs = new ImageEventArgs();
            ImageEditingStarted?.Invoke(this, imageEventArgs);

            if (imageEventArgs.Pictures == null)
                return;
            ;
            foreach (var image in imageEventArgs.Pictures)
            {
                EditedArticle.Images.Add(image);
            }
        }

        private void DeleteImage(PictureDTO image)
        {
            if (image == null)
                return;


            ConfirmationMessageEventArgs confirmationMessage = new ConfirmationMessageEventArgs("Are you sure, you want to delete this image?");
            OnConfirmationMessageApplication(confirmationMessage);

            if (confirmationMessage.Cancel)
                return;

            EditedArticle.Images.Remove(image);
        }
    }
}
