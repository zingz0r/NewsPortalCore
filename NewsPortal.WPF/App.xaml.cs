using System;
using System.Windows;
using Microsoft.Win32;
using NewsPortal.WPF.Models;
using NewsPortal.WPF.Persistences;
using NewsPortal.WPF.ViewModels;
using NewsPortal.WPF.ViewModels.EventArgumentums;
using NewsPortal.WPF.ViewModels.Helpers;
using NewsPortal.WPF.Views;

namespace NewsPortal.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NewsPortalModel _model;

        private LoginViewModel _loginViewModel;
        private MainViewModel _mainViewModel;

        private LoginWindow _loginView;
        public MainWindow _mainView;
        private ArticleEditorWindow _editorView;
        private NewsPortalPersistence _persistance;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
            Exit += new ExitEventHandler(App_Exit);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            CreateLoginInstance();
        }

        private void CreateLoginInstance()
        {
            _persistance = new NewsPortalPersistence("http://localhost:2802/");
            _model = new NewsPortalModel(_persistance);

            _loginViewModel = new LoginViewModel(_model);
            _loginViewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);
            _loginViewModel.LoginSuccess += new EventHandler(ViewModel_LoginSuccess);
            _loginViewModel.LoginFailed += new EventHandler(ViewModel_LoginFailed);
            _loginViewModel.MessageApplication += new EventHandler<MessageEventArgs>(ViewModel_MessageInvoked);

                _loginView = new LoginWindow();
            _loginView.DataContext = _loginViewModel;
            _loginView.Show();
        }

        public async void App_Exit(object sender, ExitEventArgs e)
        {
            if (_model.IsUserLoggedIn)
            {
                await _model.LogoutAsync();
            }

            _loginView?.Close();
            _mainView?.Close();
        }

        private void ViewModel_ExitApplication(object sender, System.EventArgs e)
        {
            Shutdown();
        }

        private async void ViewModel_LogoutApplication(object sender, System.EventArgs e)
        {
            if (_model.IsUserLoggedIn)
            {
                await _model.LogoutAsync();
            }

            CreateLoginInstance();

            _mainView?.Close();
        }

        private void ViewModel_LoginSuccess(object sender, EventArgs e)
        {
            _mainViewModel = new MainViewModel(_model);

            _mainViewModel.ArticleEditingStarted += new EventHandler(MainViewModel_ArticleEditingStarted);
            _mainViewModel.ArticleEditingFinished += new EventHandler(MainViewModel_ArticleEditingFinished);
            _mainViewModel.ImageEditingStarted += new EventHandler<ImageEventArgs>(MainViewModel_ImageEditingStarted);

            _mainViewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);
            _mainViewModel.LogoutApplication += new EventHandler(ViewModel_LogoutApplication);

            _mainViewModel.MessageApplication += new EventHandler<MessageEventArgs>(ViewModel_MessageInvoked);
            _mainViewModel.ConfirmationMessageApplication += new EventHandler<ConfirmationMessageEventArgs>(ViewModel_ConfirmationMessageInvoked);
            
            _mainView = new MainWindow();
            _mainView.DataContext = _mainViewModel;
            _mainView?.Show();

            _loginView.Close();
        }

        private void MainViewModel_ArticleEditingStarted(object sender, EventArgs e)
        {
            _editorView = new ArticleEditorWindow();
            _editorView.DataContext = _mainViewModel;
            _editorView.ShowDialog();
        }
        private void MainViewModel_ArticleEditingFinished(object sender, EventArgs e)
        {
            _editorView.Close();
        }

        private void MainViewModel_ImageEditingStarted(object sender, ImageEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = "Image Files|*.jpg;*.jpeg;*.bmp;*.tif;*.gif;*.png;",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                };

                var result = dialog.ShowDialog();

                if (result == true)
                {
                    e.LargeImageData = ImageHelper.OpenAndResize(dialog.FileName, 700);
                    e.SmallImageData = ImageHelper.OpenAndResize(dialog.FileName, 350);
                }
            }
            catch { }
        }

        private static void ViewModel_LoginFailed(object sender, EventArgs e)
        {
            MessageBox.Show("Login Failed!", "News Portal", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        
        private void ViewModel_MessageInvoked(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

        }
        private void ViewModel_ConfirmationMessageInvoked(object sender, ConfirmationMessageEventArgs e)
        {
            var result = MessageBox.Show(e.Message, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        
    }
}
