using Microsoft.AspNetCore.Identity;
using NewsPortal.Data.Entity;

namespace NewsPortal.WebAPI.Models
{
    public static class DbInitializer
    {
        private static NewsPortalContext _context;
        private static UserManager<User> _userManager;
        private static RoleManager<IdentityRole<int>> _roleManager;

        public static void Initialize(NewsPortalContext context,
            UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager,
            string imageDirectory)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

            //_context.Database.Migrate();

            //if (!_context.Users.Any())
            //{
                SeedUsers();
            //}
        }

        private static void SeedUsers()
        {
            var zingUser = new User
            {
                UserName = "zingz0r1",
                Name = "Tóth Tamás",
                Email = "zingz0r@tothnet.hu",
                PhoneNumber = "+36308307074",
            };
            var zingPassword = "Test123Test123";
            var zingRole = new IdentityRole<int>("administrator");

            var result1 = _userManager.CreateAsync(zingUser, zingPassword).Result;
            var result2 = _roleManager.CreateAsync(zingRole).Result;
            var result3 = _userManager.AddToRoleAsync(zingUser, zingRole.Name).Result;

            var testUser = new User
            {
                UserName = "testJanos",
                Name = "Teszt János",
                Email = "testjano@tothnet.hu",
                PhoneNumber = "+36304856585",
            };
            var testPassword = "Test123Test123";
            var testRole = new IdentityRole<int>("administrator");

            var result4 = _userManager.CreateAsync(testUser, testPassword).Result;
            var result5 = _roleManager.CreateAsync(testRole).Result;
            var result6 = _userManager.AddToRoleAsync(testUser, testRole.Name).Result;
        }
    }
}
