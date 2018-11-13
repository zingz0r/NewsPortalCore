using System;
using System.Threading.Tasks;
using NewsPortal.WPF.Persistences;

namespace NewsPortal.WPF.Models
{
    class NewsPortalModel : INewsPortalModel
    {
        private readonly INewsPortalPersistence _persistence;
        public bool IsUserLoggedIn { get; private set; }

        public NewsPortalModel(INewsPortalPersistence persistence)
        {
            IsUserLoggedIn = false;
            _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
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
