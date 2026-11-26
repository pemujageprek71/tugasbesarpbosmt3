using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TA_sem3.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signIn;
    public AccountController(SignInManager<IdentityUser> signIn) => _signIn = signIn;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, bool rememberMe = false, string? returnUrl = null)
    {
        var result = await _signIn.PasswordSignInAsync(email ?? "", password ?? "", rememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            var user = await _signIn.UserManager.FindByNameAsync(email!);
            if (user != null && await _signIn.UserManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("Dashboard", "Admin");
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        ViewData["Error"] = "Email atau kata sandi salah.";
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();
}