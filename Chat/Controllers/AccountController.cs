using Chat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

            [HttpPost]  
        public async Task<IActionResult> Login(AppUser appUser)
        {
            AppUser user = await _userManager.FindByNameAsync(appUser.UserName);
          var result =   await _signInManager.PasswordSignInAsync(user, appUser.PasswordHash, true, true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password incorrect");
                return View();
            }
            
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
