using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.Data.Entity;

namespace NewsPortal.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet("login")]
        public JsonResult Login()
        {
            return Json(new { message = "You need to log in first." });
        }

        [HttpGet("login/{userName}/{userPassword}")]
        public async Task<JsonResult> Login(String userName, String userPassword)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(userName, userPassword, false, false);
                if (!result.Succeeded)
                    return Json(new { message = "Login Failed", response="false" });

                return Json(new { message = "Login Successful", response = "true" });
            }

            // TODO: visszaadni a usert json helyett,
            catch
            {
                return Json(new { message = "Something Happened! :(", response = "false" });
            }
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
