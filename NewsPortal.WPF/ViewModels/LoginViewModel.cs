using System;
using System.Windows.Controls;
using NewsPortal.WPF.Models;
using NewsPortal.WPF.Persistences;
using NewsPortal.WPF.Resources;
using NewsPortal.WPF.ViewModels.BaseViewModel;

namespace NewsPortal.WPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly INewsPortalModel _model;
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand LoginCommand { get; private set; }
        public string UserName { get; set; }
        public event EventHandler ExitApplication;
        public event EventHandler LoginSuccess;
        public event EventHandler LoginFailed;
        public object LoginImage => ResourceManager.GetPngImage(Images.icon_lock);
        public object LockImage => ResourceManager.GetPngImage(Images.icon_lock);
        public LoginViewModel(INewsPortalModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));

            UserName = string.Empty;

            ExitCommand = new DelegateCommand(param => OnExitApplication());
            LoginCommand = new DelegateCommand(param => LoginAsync(param as PasswordBox));
        }

        private async void LoginAsync(PasswordBox passwordBox)
        {
            if (passwordBox == null)
            {
                return;
            }

            try
            {
                bool result = await _model.LoginAsync(UserName, passwordBox.Password);

                if (result)
                    OnLoginSuccess();
                else
                    OnLoginFailed();
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("No connection to the api.");
            }
        }

        private void OnLoginSuccess()
        {
            LoginSuccess?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitApplication()
        {
            ExitApplication?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoginFailed()
        {
            LoginFailed?.Invoke(this, EventArgs.Empty);
        }

    }
}
