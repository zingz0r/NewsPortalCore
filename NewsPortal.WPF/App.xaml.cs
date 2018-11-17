using System;
using System.Windows;
using NewsPortal.WPF.Models;
using NewsPortal.WPF.Persistences;
using NewsPortal.WPF.ViewModels;
using NewsPortal.WPF.ViewModels.EventArgumentums;
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

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
            Exit += new ExitEventHandler(App_Exit);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new NewsPortalModel(new NewsPortalPersistence("http://localhost:2802/"));

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

            _mainView?.Hide();
            if (_loginViewModel == null)
            {
                _loginViewModel = new LoginViewModel(_model);
                _loginViewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);
                _loginViewModel.LoginSuccess += new EventHandler(ViewModel_LoginSuccess);
                _loginViewModel.LoginFailed += new EventHandler(ViewModel_LoginFailed);
                _loginViewModel.MessageApplication += new EventHandler<MessageEventArgs>(ViewModel_MessageInvoked);
            }

            if (_loginView == null)
            {
                _loginView = new LoginWindow();
                _loginView.DataContext = _loginViewModel;
            }

            _loginView.Show();
        }

        private void ViewModel_LoginSuccess(object sender, EventArgs e)
        {
            _mainViewModel = new MainViewModel(_model);
            _mainViewModel.MessageApplication += new EventHandler<MessageEventArgs>(ViewModel_MessageInvoked);
            _mainViewModel.ArticleEditingStarted += new EventHandler(MainViewModel_ArticleEditingStarted);
            _mainViewModel.ArticleEditingFinished += new EventHandler(MainViewModel_ArticleEditingFinished);
            _mainViewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);
            _mainViewModel.LogoutApplication += new EventHandler(ViewModel_LogoutApplication);

            if (_mainView == null)
                _mainView = new MainWindow();
            _mainView.DataContext = _mainViewModel;
            _mainView?.Show();

            _loginView.Hide();
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

        private static void ViewModel_LoginFailed(object sender, EventArgs e)
        {
            MessageBox.Show("Login Failed!", "News Portal", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        
        private void ViewModel_MessageInvoked(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

        }
    }
}
